using FluentValidation;

namespace Common.DTO.Validator
{
    public class BaseDTOValidator : AbstractValidator<BaseDTO>
    {
        public BaseDTOValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required.")
                                .Length(1, 100).WithMessage("Name length can't be more than 100 characters.");
        }
    }

    public class CenterDTOValidator : AbstractValidator<CenterDTO>
    {
        public CenterDTOValidator()
        {
            Include(new BaseDTOValidator());

            RuleFor(x => x.Address).NotEmpty().WithMessage("Address is required.");
            RuleFor(x => x.ZipCode).Matches(@"^\d{5}(-\d{4})?$").WithMessage("Invalid zip code format.");
            RuleFor(x => x.PhoneNumber).NotEmpty().WithMessage("Phone number is required.")
                                       .Matches(@"^\+[1-9]\d{1,14}$").WithMessage("Invalid phone number format.");
            RuleFor(x => x.Email).NotEmpty().WithMessage("Email is required.")
                                 .EmailAddress().WithMessage("Invalid email address format.");
        }
    }

    public class CommodityDTOValidator : AbstractValidator<CommodityDTO>
    {
        public CommodityDTOValidator()
        {
            Include(new BaseDTOValidator());

            RuleFor(x => x.Price).GreaterThan(0).WithMessage("Price must be greater than 0.");
            RuleFor(x => x.Description).Length(0, 500).WithMessage("Description length can't be more than 500 characters.");
            RuleFor(x => x.Quantity).GreaterThanOrEqualTo(1).WithMessage("Quantity must be at least 1.");
        }
    }
}
