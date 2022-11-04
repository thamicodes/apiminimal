class Base
{
  private int baseField;

  public override bool Equals(object other)
  {
    if (base.Equals(other)) // Okay; base is object
    {
      return true;
    }

    return this.baseField == ((Base)other).baseField;
  }
  public void Test()
  {
    var fruitBasket = new List<Fruit>();
    fruitBasket.Add(new Orange());
    fruitBasket.Add(new Orange());
    // fruitBasket.Add(new Apple());  // uncommenting this line will make both foreach below throw an InvalidCastException

    foreach (Fruit fruit in fruitBasket)
    {
      var orange = (Orange)fruit; // This "explicit" conversion is hidden within the foreach loop below
     
    }

    foreach (Orange orange in fruitBasket) // Noncompliant
    {
     
    }
  }
}
public abstract class Fruit{}
public class Orange : Fruit{}
