namespace XamarinPhoneContact.View;

public class ScaleAnimationTriggerAction : TriggerAction<Image>
{
  protected override async void Invoke(Image image)
  {
    image.Opacity = 1;
    image.Scale = 0.2;
    await image.ScaleToAsync(1.2, 150, Easing.SpringOut);
    await image.ScaleToAsync(1.0, 100, Easing.SpringIn);
  }
}

public class ScaleOutAnimationTriggerAction : TriggerAction<Image>
{
  protected override async void Invoke(Image image)
  {
    await image.ScaleToAsync(1.2, 100, Easing.CubicIn);
    await image.ScaleToAsync(0, 200, Easing.CubicOut);
    image.Opacity = 0;
  }
}
