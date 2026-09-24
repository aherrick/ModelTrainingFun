import runpy
import sys
from pathlib import Path

BASE_MODEL = "Qwen/Qwen2.5-3B-Instruct"


def export_base(output_dir: str) -> None:
    _build_onnx(["-m", BASE_MODEL], output_dir)


def fine_tune(questions: list[str], answers: list[str], output_dir: str, epochs: int) -> None:
    import torch
    from datasets import Dataset
    from peft import LoraConfig
    from transformers import AutoModelForCausalLM, AutoTokenizer
    from trl import SFTConfig, SFTTrainer

    work = Path(output_dir).parent / "_training"
    tokenizer = AutoTokenizer.from_pretrained(BASE_MODEL)
    model = AutoModelForCausalLM.from_pretrained(BASE_MODEL, dtype=torch.float32)

    # Conversational prompt/completion format: loss is computed on the answers only.
    data = Dataset.from_dict({
        "prompt": [[{"role": "user", "content": q}] for q in questions],
        "completion": [[{"role": "assistant", "content": a}] for a in answers],
    })

    trainer = SFTTrainer(
        model=model,
        processing_class=tokenizer,
        train_dataset=data,
        peft_config=LoraConfig(r=16, lora_alpha=32, target_modules="all-linear", task_type="CAUSAL_LM"),
        args=SFTConfig(
            output_dir=str(work / "checkpoints"),
            num_train_epochs=epochs,
            learning_rate=2e-4,
            bf16=False,  # TRL defaults to bf16, which needs a GPU; we train fp32 on CPU
            per_device_train_batch_size=8,
            logging_steps=5,
            save_strategy="no",
            report_to="none",
        ),
    )
    trainer.train()

    # Bake the LoRA weights into a brand-new standalone model, then export it to ONNX.
    merged_dir = work / "merged"
    trainer.model.merge_and_unload().save_pretrained(merged_dir)
    tokenizer.save_pretrained(merged_dir)
    _build_onnx(["-i", str(merged_dir)], output_dir)


def _build_onnx(source_args: list[str], output_dir: str) -> None:
    cache = Path(output_dir).parent / "_cache"
    saved = sys.argv
    sys.argv = ["builder", *source_args, "-o", output_dir, "-p", "fp32", "-e", "cpu", "-c", str(cache),
                "--extra_options", "hf_token=false"]  # public model; builder otherwise demands a login
    try:
        runpy.run_module("onnxruntime_genai.models.builder", run_name="__main__")
    finally:
        sys.argv = saved
