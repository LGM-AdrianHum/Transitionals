# Transitionals
WPF Transitionals

Transitionals is a framework for building and using WPF transitions which provide an easy way to switch between UI views in a rich and animated way. Think of transitions for applications in the same way you think of transitions for video editing. Wipe, Cut, Dissolve, Star, Blinds and 3D Rotating Cube are all examples of transitions supported by the Transitionals framework.

The best way to get started with the Transitionals framework is to download and take a look at the TransitionalsHelp file. You can find it on the Releases tab and it includes a pretty comprehensive Getting Started guide. You can also download the binary archive which includes two sample projects. 

Transitionals was originally compiled with in Visual Studio 2008 against .Net Framework 3.5.
It has been recompiled with Visual Studio 2015 against .Net Framework 4.5.2

Transitionals brings a portion of the Microsoft Acropolis research project to the community and continues that development. THANK YOU to the Acropolis team for making this project possible. 

The original project can be found here: https://transitionals.codeplex.com/

The Transitions library integrates into your WPF application at the XAML level.  Adding a transition effect is easy:

Add a reference to the Transitionals.dll assembly to your project
Add the appropriate XML namespaces to your WPF document:
```
xmlns:trans="clr-namespace:Transitionals;assembly=Transitionals"
xmlns:transc="clr-namespace:Transitionals.Controls;assembly=Transitionals"
xmlns:transt="clr-namespace:Transitionals.Transitions;assembly=Transitionals"
xmlns:refl="clr-namespace:System.Reflection;assembly=mscorlib"
```
Add a TransitionElement object to the XAML of your window or user control that specifies the transformations you wantinside the TransitionElement.Transition property:
```
<transc:TransitionElement x:Name="TransitionBox">
    <transc:TransitionElement.Transition>
        <transt:RotateTransition Angle="45" />
    </transc:TransitionElement.Transition>
</transc:TransitionElement>
```
Alternatively, you can set the TransactionElement.TransactionSelector property to something like the built-in RandomTransitionSelector class to choose between a set of your favorite Transitions:
```
<transc:TransitionElement x:Name="TransitionBox">
    <transc:TransitionElement.TransitionSelector>
        <trans:RandomTransitionSelector>
            <transt:DoorTransition/>
            <transt:DotsTransition/>
            <transt:RotateTransition Angle="45" />
            <transt:RollTransition/>
        </trans:RandomTransitionSelector>
    </transc:TransitionElement.TransitionSelector>
</TransitionElement>
```
Now all that's left is to assign the pieces of content you want managed by the TransitionElement.  These go in the Content property as shown in this sample.  In this case, we've added 2 button controls (AButton and BButton) and are using their Button.Click events to swap between two separate User Controls:
UserControlA userControlA = new UserControlA();
UserControlB userControlB = new UserControlB();
``` 
private void AButton_Click(object sender, RoutedEventArgs e)
{
    TransitionBox.Content = userControlA;
}
 
private void BButton_Click(object sender, RoutedEventArgs e)
{
    TransitionBox.Content = userControlB;
}
```
Presto!  When you run the application, the transition will take place when you click between the buttons.
