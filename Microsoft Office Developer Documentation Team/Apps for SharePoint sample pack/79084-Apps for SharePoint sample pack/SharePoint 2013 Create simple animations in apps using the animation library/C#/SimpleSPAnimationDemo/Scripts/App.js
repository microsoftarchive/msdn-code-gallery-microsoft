

// This code runs when the DOM is ready and creates a context object which is needed to use the SharePoint object model
$(function () {  
  $("#cmdHidePhoto").click(onHidePhoto);
  $("#cmdShowPhoto").click(onShowPhoto);
  $("#cmdEnlargePhoto").click(onEnlargePhoto);
  $("#cmdChangeOpacity").click(onChangeOpacity);
});

function onHidePhoto() {
  var photo1 = document.getElementById("photo1");
  SPAnimationUtility.BasicAnimator.FadeOut(photo1);
}

function onShowPhoto() {
  var photo1 = document.getElementById("photo1");
  SPAnimationUtility.BasicAnimator.FadeIn(photo1);
}


function onEnlargePhoto() {
  var photo1 = document.getElementById("photo1");
  SPAnimationUtility.BasicAnimator.Resize(photo1, 500, 500);
}

function onChangeOpacity() {

  var photo1 = document.getElementById("photo1");

  var state = new SPAnimation.State();
  state.SetAttribute(SPAnimation.Attribute.Opacity, 0.2);
  var animation = new SPAnimation.Object(SPAnimation.ID.Basic_Opacity, 500, photo1, state);

  animation.RunAnimation();


}
