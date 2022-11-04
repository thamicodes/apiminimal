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
}