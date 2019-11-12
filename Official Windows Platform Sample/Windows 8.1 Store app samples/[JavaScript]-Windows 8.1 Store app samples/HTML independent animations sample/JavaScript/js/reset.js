//Once a scenario is run all the animating property values are removed and reset so the user can try new values
var eventcount2 = 0;
// This function is being used to reset CSS Transition scenarios
function resetPage2(events) {
    var element4 = document.getElementById("scenarioSelect");
    element4.style.transition = 'none';
    element4.style.transform = '';
    element4.style.transformOrigin = '50%';

    resetelement.style.transition = 'none';
    resetelement.style.opacity = '1';
    resetelement.style.transform = '';
    resetelement.style.transformOrigin = '50%';

    element4.addEventListener('transitionend', resetPage2, false);
}

// This function is being used to reset the CSS Animation scenarios
function resetPage(events) {
    eventcount2++;
    if (eventcount2 === 2) {
        eventcount2 = 0;
        resetelement.style.transition = 'none';
        resetelement.style.opacity = '1';
        resetelement.style.transform = ''; 
        resetelement.style.transformOrigin = '50%';
    }
    resetelement.style.animationName = '';
    resetelement.removeEventListener('animationend', resetPage, false);
}
