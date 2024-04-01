using System.ComponentModel.DataAnnotations;

namespace Common.DTO
{
    public class BaseDTO
    {
        public string? Id { get; set; }
        public string? Name { get; set; }

        public DateTime CreatedAt { get; set; }
        public string? CenterId { get; set; }
    }

    public class CenterDTO : BaseDTO
    {
        public string? Address { get; set; }

        public string? ZipCode { get; set; }

        public string? PhoneNumber { get; set; }

        public string? Email { get; set; }
    }

    public class CommodityDTO : BaseDTO
    {
        public decimal? Price { get; set; }

        public string? Description { get; set; }

        public int? Quantity { get; set; }

        public string? ImageUrl { get; set; }

        public string? CategoryId { get; set; }
    }
    public class StatusDTO : BaseDTO
    {

    }
 
    public class CreateDTO : BaseDTO
    {

    }
    public class UpdateDTO : BaseDTO
    {

    }
    public class DeleteDTO : BaseDTO
    {

    }
}
