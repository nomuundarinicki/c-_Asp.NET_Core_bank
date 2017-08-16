
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace bank.Models
{
  public class Transaction : BaseEntity
  {
    public int TransactionId { get; set; }
    [Required]
    [RegularExpression(@"^\d+\.\d{0,2}$")]
    public Double Amount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public int UserId { get; set; }
    public User Accountholder { get; set; }
  }
}