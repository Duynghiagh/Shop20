using System.ComponentModel.DataAnnotations;

namespace ShoppingDemo.Repository.Valication
{
    public class FileExtenstionAttribute: ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if(value is IFormFile file)
            {
                var extension = Path.GetExtension(file.FileName);
                string[] extensions = { "jpg", "png","jpeg"};
                bool result =extension.Any(x=>extension.EndsWith(x));
                if (!result)
                {
                    return new ValidationResult("Allowed extensions are jpg, png or pjeg");
                }
            }
            return ValidationResult.Success;
        }
    }
}
