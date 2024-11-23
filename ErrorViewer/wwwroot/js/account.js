let accounts = []

function closeAddUserModal() {
    let form = $('#addUserForm');
    form.trigger("reset");
}

function addUserSubmit() {
    
    let form = $('#addUserForm');
    let data = {
        username: form.find("#username-am").val(),
        password: form.find("#password-am").val(),
        isAdmin: form.find("#isAdmin-am").is(":checked"),
        isBanned: form.find("#isBanned-am").is(":checked"),
    };
    
    $.ajax({
        url: "/Auth/RegisterUser/",
        type: "POST",
        data: $.param(data),
        success: function(response){
            console.log(response);
            
            let anyError = $("#add-user-error");
            if(anyError != null){
                anyError.remove();
            }
            
            if(response.error != null) {
                let error = response.error;

                if(error === "NotAdmin"){
                    $("#addUserFooter").append("<p class='text-danger' id=\"add-user-error\">Only admins can add users!</p>");
                }
                else if(error === "UsernameTaken"){
                    $("#addUserFooter").append("<p class='text-danger' id=\"add-user-error\">Username is taken!</p>");
                }
                else if (error === "UsernameTooShort"){
                    $("#addUserFooter").append("<p class='text-danger' id=\"add-user-error\">Username is too short! It needs to be at least 3 characters long</p>");
                }
                else if (error === "PasswordTooShort"){
                    $("#addUserFooter").append("<p class='text-danger' id=\"add-user-error\">Password is too short! It needs to be at least 8 characters long</p>");
                }
                else {
                    $("#addUserFooter").append("<p class='text-danger' id=\"add-user-error\">An error has occured while adding user!</p>");
                }
                
                return;
            }
            
            showToast("createAccountToast");
            form.trigger("reset");
            loadAccounts();
            $("#addUserModal").modal("toggle");
        },
        
    })
    
}

function loadAccounts() {
    
    let tableBody = $("#accountsTable tbody");

    $.ajax({
        url: "/Auth/GetUsers/",
        type: "GET",
        success: function(data) {
            console.log(data);
            accounts = data;
            
            tableBody.empty();
            data.forEach(function (account) {
                
                let row = `
                         <tr>
                            <td>
                                ${account.username}
                            </td>
                            <td>
                                ${account.isAdmin ? "Yes" : "No"}
                            </td>
                            <td>
                                ${account.isBanned ? "Yes" : "No"}
                            </td>
                            <td>
                                ${account.createdAt}
                            </td>
                            <td>
                                <button class="btn btn-dark py-0" onclick='banAccount("${account.username}")'>
                                    <i class="bi bi-x"></i>
                                    ${account.isBanned ? "Unblock" : "Block"}
                                </button>
                                <button class="btn btn-dark py-0" onclick='editAccount("${account.username}")'>
                                    <i class="bi bi-pencil"></i>
                                    Edit
                                </button>
                                <button class="btn btn-danger py-0" onclick='removeAccount("${account.username}")'>
                                    <i class="bi bi-trash"></i>
                                    Remove
                                </button>
                            </td>
                        </tr>
                `;
                
                if(account.username !== "admin") {
                    tableBody.append(row);
                }
            });
            
            
            console.log("Accounts loaded");
        }
    })
    


}

function banAccount(username) {
    
    $.ajax({
        url: "/Auth/BanUser/" + username,
        type: "GET",
        success: function(response){
            console.log(response);
            
            if(response.error != null) {

                if(response.error === "NotAdmin") {
                    showToast("accessDenitedToast");
                } 
                else if(response.error === "UserBanned") {
                    showToast("accessDenitedToast");
                }
                else if(response.error === "UserNotFound") {
                    showToast("accountNotFoundToast");
                }
                else {
                    showToast("error");
                }
                
                return;
            }
            
            showToast("banAccountToast");
            loadAccounts();
        },
    })
}

function editAccount(username) {
    
    console.log('Editing account: ' + username);
    let form = $('#editUserForm');
    let modal = $("#editUserModal");
    let account = accounts.find(account => account.username === username);
    
    form.find("#username-em").val(account.username);
    form.find("#isAdmin-em").prop("checked", account.isAdmin);
    form.find("#isBanned-em").prop("checked", account.isBanned);
    
    modal.modal("toggle");
}

function editUserSubmit() {
    
    let modal = $("#editUserModal");
    let form = $('#editUserForm');

    let data = {
        username: form.find("#username-em").val(),
        passwordToChange: form.find("#passwordToChange-em").val(),
        isAdmin: form.find("#isAdmin-em").is(":checked"),
        isBanned: form.find("#isBanned-em").is(":checked"),
    }
    
    $.ajax({
        url: "/Auth/EditUser/",
        type: "POST",
        data: $.param(data),
        success: function (response){
            
            let anyError = $("#edit-user-error");
            if(anyError != null){
                anyError.remove();
            }
            
            if(response.error != null) {
                let error = response.error;
                
                if (error === "UserNotFound") {
                    $("#editUserFooter").append("<p class='text-danger' id=\"edit-user-error\">User not found!</p>");
                }
                else if(error === "NotAdmin"){
                    $("#editUserFooter").append("<p class='text-danger' id=\"edit-user-error\">Only admins can edit users!</p>");
                }
                else if(error === "PasswordTooShort"){
                    $("#editUserFooter").append("<p class='text-danger' id=\"edit-user-error\">Password is too short! It needs to be at least 8 characters long</p>");
                }
                else {
                    $("#editUserFooter").append("<p class='text-danger' id=\"edit-user-error\">An error has occured while editing user!</p>");
                }
                
                return;
            }
            
            form.trigger("reset");
            loadAccounts();
            modal.modal("toggle");
            showToast("editAccountToast");
        },
        error: function (response){
            console.log(response);
            showToast("error");
        }
    });
    
    
}

function closeEditUserModal() {
    let form = $('#editUserForm');
    form.trigger("reset");
}

function removeAccount(username) {
    $.ajax({
        url: "/Auth/RemoveUser/" + username,
        type: "GET",
        success: function(response){
            console.log(response);

            if(response.error != null) {

                if(response.error === "NotAdmin") {
                    showToast("accessDenitedToast");
                }
                else if(response.error === "UserBanned") {
                    showToast("accessDenitedToast");
                }
                else if(response.error === "UserNotFound") {
                    showToast("accountNotFoundToast");
                }
                else {
                    showToast("error");
                }

                return;
            }

            showToast("removeAccountToast");
            loadAccounts();
        },
    })
}

function changePassword(){
    let form = $('#changePasswordForm');
    
    let data = form.serialize();
    
    
    
    $.ajax({
        url: "/Auth/ChangePassword/",
        type: "POST",
        data: data,
        success: function(response){
            console.log(response);

            let anyError = $("#change-password-error");
            if(anyError != null){
                anyError.remove();
            }
            
            if(response.error != null) {
                let error = response.error;
                
                if(error === "PasswordTooShort"){
                    $("#changePasswordFooter").append("<p class='text-danger' id=\"change-password-error\">Password is too short! It needs to be at least 8 characters long</p>");
                } 
                else if (error === "PasswordSameAsOld"){
                    $("#changePasswordFooter").append("<p class='text-danger' id=\"change-password-error\">New password can't be the same as the old one!</p>");
                }
                else if (error === "PasswordIncorrect"){
                    $("#changePasswordFooter").append("<p class='text-danger' id=\"change-password-error\">Old password is incorrect!</p>");
                }
                else {
                    $("#changePasswordFooter").append("<p class='text-danger' id=\"change-password-error\">An error has occured while changing password!</p>");
                }
                
                return;
            }

            
            showToast("passwordChangedToast");
            form.trigger("reset");
            
        },
        error: function(response){
            $("#changePasswordFooter").append("<p class='text-danger' id=\"change-password-error\">An error has occured while changing password!</p>");
            
            console.log(response);
            showToast("error");
        }
    });
    
}


window.addEventListener('load', function () {
    loadAccounts();
})