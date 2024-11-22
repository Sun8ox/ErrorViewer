// Javascript makes me wanna put toaster to the bath w me
function showToast(toastName) {

    let toast = document.getElementById(toastName)
    if(toast){
        bootstrap.Toast.getOrCreateInstance(toast).show();
    } else {
        console.log("Toast not found!");
    }
}