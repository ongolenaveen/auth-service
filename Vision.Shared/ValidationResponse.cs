using System.ComponentModel.DataAnnotations;

namespace Vision.Shared
{
    public class ValidationResponse
    {
        public bool IsValid { get; set; }

        public List<ValidationResult> Results { get; set; } = [];
    }
}
