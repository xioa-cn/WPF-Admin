using System;
using System.ComponentModel.DataAnnotations;

namespace Xioa.Admin.Core.Views.DataValidator.Validations;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
public class FloatRuleAttribute : ValidationAttribute
{
    public float Min { get; }
    public float Max { get; }

    public FloatRuleAttribute(float min, float max)
    {
        Min = min;
        Max = max;
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        // 如果值为空，返回成功（因为通常会配合Required特性使用）
        if (value == null)
        {
            return ValidationResult.Success;
        }

        // 尝试转换为float
        if (!float.TryParse(value.ToString(), out float floatValue))
        {
            return new ValidationResult($"请输入有效的数字");
        }

        // 检查范围
        if (floatValue < Min || floatValue > Max)
        {
            return new ValidationResult(ErrorMessage ?? $"数值必须在 {Min} 到 {Max} 之间");
        }

        return ValidationResult.Success;
    }
} 