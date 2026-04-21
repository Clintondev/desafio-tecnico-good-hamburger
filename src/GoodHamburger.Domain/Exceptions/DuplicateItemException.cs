namespace GoodHamburger.Domain.Exceptions;

public class DuplicateItemException : DomainException
{
    public DuplicateItemException(string itemName)
        : base($"O pedido ja contem '{itemName}'. Cada pedido permite apenas um item por categoria.")
    {
    }
}
