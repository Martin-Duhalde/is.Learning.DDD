namespace CarRental.Domain.Entities;

public interface IEntity
{
    Guid            /**/ Id             /**/ { get; set; }
    bool            /**/ IsActive       /**/ { get; set; }
    int             /**/ Version        /**/ { get; set; } /// Control de concurrencia con versión incremental
}
