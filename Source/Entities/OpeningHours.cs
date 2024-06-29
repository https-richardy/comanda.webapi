namespace Comanda.WebApi.Entities;

public sealed class OpeningHours : Entity
{
    public DayOfWeek DayOfWeek { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }

    public OpeningHours()
    {
        /*
            Default parameterless constructor included due to Entity Framework Core not setting navigation properties
            when using constructors. For more information, see: https://learn.microsoft.com/pt-br/ef/core/modeling/constructors
        */
    }

    public OpeningHours(DayOfWeek dayOfWeek, TimeSpan startTime, TimeSpan endTime)
    {
        DayOfWeek = dayOfWeek;
        StartTime = startTime;
        EndTime = endTime;
    }
}