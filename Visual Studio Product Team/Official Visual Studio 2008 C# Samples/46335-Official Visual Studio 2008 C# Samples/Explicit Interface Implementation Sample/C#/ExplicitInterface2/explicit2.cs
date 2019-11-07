//Copyright (C) Microsoft Corporation.  All rights reserved.

// explicit2.cs
// Declare the English units interface:
interface IEnglishDimensions 
{
   float Length();
   float Width();
}
// Declare the metric units interface:
interface IMetricDimensions 
{
   float Length();
   float Width();
}
// Declare the "Box" class that implements the two interfaces:
// IEnglishDimensions and IMetricDimensions:
class Box : IEnglishDimensions, IMetricDimensions 
{
   float lengthInches;
   float widthInches;
   public Box(float length, float width) 
   {
      lengthInches = length;
      widthInches = width;
   }
// Explicitly implement the members of IEnglishDimensions:
   float IEnglishDimensions.Length() 
   {
      return lengthInches;
   }
   float IEnglishDimensions.Width() 
   {
      return widthInches;      
   }
// Explicitly implement the members of IMetricDimensions:
   float IMetricDimensions.Length() 
   {
      return lengthInches * 2.54f;
   }
   float IMetricDimensions.Width() 
   {
      return widthInches * 2.54f;
   }
   public static void Main() 
   {
      // Declare a class instance "myBox":
      Box myBox = new Box(30.0f, 20.0f);
      // Declare an instance of the English units interface:
      IEnglishDimensions eDimensions = (IEnglishDimensions) myBox;
      // Declare an instance of the metric units interface:
      IMetricDimensions mDimensions = (IMetricDimensions) myBox;
      // Print dimensions in English units:
      System.Console.WriteLine("Length(in): {0}", eDimensions.Length());
      System.Console.WriteLine("Width (in): {0}", eDimensions.Width());
      // Print dimensions in metric units:
      System.Console.WriteLine("Length(cm): {0}", mDimensions.Length());
      System.Console.WriteLine("Width (cm): {0}", mDimensions.Width());
   }
}

