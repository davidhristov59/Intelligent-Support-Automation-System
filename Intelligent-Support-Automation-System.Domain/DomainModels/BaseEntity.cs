using System.ComponentModel.DataAnnotations;

namespace Intelligent_Support_Automation_System.Domain.DomainModels;

public class BaseEntity
{
    [Key]
    public Guid Id { get; set; }
}