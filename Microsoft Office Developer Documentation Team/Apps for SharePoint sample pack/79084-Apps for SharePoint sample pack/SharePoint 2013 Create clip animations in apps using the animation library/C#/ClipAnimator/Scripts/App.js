var delta = 50;
var positionX = 0;
var positionY = 0;

// This code runs when the DOM is ready and creates a context object which is needed to use the SharePoint object model
$(function () {
  $("#cmdMoveLeft").click(onMoveLeft);
  $("#cmdMoveRight").click(onMoveRight);
  $("#cmdMoveDown").click(onMoveDown);
  $("#cmdMoveUp").click(onMoveUp);
  $("#cmdReset").click(onReset);
});


function movePhoto() {

  var photo1 = document.getElementById("photo1");
 
  var state = new SPAnimation.State();
  state.SetAttribute(SPAnimation.Attribute.PositionX, positionX);
  state.SetAttribute(SPAnimation.Attribute.PositionY, positionY);

  var animation = new SPAnimation.Object(SPAnimation.ID.Basic_Move, 0, photo1, state);

  animation.RunAnimation();

}

function onMoveLeft() {
  positionX = positionX + delta;
  movePhoto();
}

function onMoveRight() {
  positionX = positionX - delta;
  movePhoto();
}

function onMoveDown() {
  positionY = positionY + delta;
  movePhoto();
}

function onMoveUp() {
  positionY = positionY - delta;
  movePhoto();
}

function onReset() {
  positionX = 0;
  positionY = 0;
  movePhoto();
}