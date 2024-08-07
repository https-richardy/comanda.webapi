namespace Comanda.WebApi.Validators;

public sealed class SettingsEditingValidator :
    AbstractValidator<SettingsEditingRequest>,
    IValidator<SettingsEditingRequest>
{
    public SettingsEditingValidator()
    {
        RuleFor(settings => settings.AcceptAutomatically)
            .NotNull().WithMessage("acceptAutomatically must be specified.");

        RuleFor(settings => settings.MaxConcurrentAutomaticOrders)
            .GreaterThan(0).WithMessage("maxConcurrentAutomaticOrders must be greater than 0.")
            .LessThanOrEqualTo(100).WithMessage("maxConcurrentAutomaticOrders must be less than or equal to 100.");

        RuleFor(settings => settings.EstimatedDeliveryTimeInMinutes)
            .GreaterThan(0).WithMessage("estimatedDeliveryTimeInMinutes must be greater than 0.")
            .LessThanOrEqualTo(180).WithMessage("estimatedDeliveryTimeInMinutes must be less than or equal to 180.");

        RuleFor(settings => settings.DeliveryFee)
            .GreaterThanOrEqualTo(0).WithMessage("deliveryFee cannot be negative.");
    }
}