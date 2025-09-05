namespace ExpenseSplit.Domain;

public interface IBaseClass
{
     Guid CreatedBy { get; set; }
     DateTime CreatedOn { get; set; }
     Guid UpdatedBy { get; set; }
     DateTime UpdatedOn { get; set; }
}
