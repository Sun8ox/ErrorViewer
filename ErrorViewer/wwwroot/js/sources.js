let sources = [];


// Nenávidím JavaScript :)

// To čo za skurveneho koňa vymyslalo javascript

// Javascript je podla mna skvelé riešenie depresie,
// pomôže ti ukončiť si život skôr.

function closeAddSourceModal() {
    $("#addSourceForm").trigger("reset");
    $("add-error").remove();
}

function closeEditSourceModal() {
    $("#editSourceForm").trigger("reset");
    $("edit-error").remove();
}

function getSources() {
    $.ajax({
        url: "/Api/getSources/",
        method: "GET",
        dataType: "json",
        success: function(data) {
            sources = data;
            console.log('Sources downloaded!');
        },
    });
}

function createSource() {
    let form = $("#addSourceForm");
    
    const data = form.serialize();
    $.ajax({
        url: "/Api/addSource/",
        method: "POST",
        data: data,
        success: function(data) {
            if(data.error != null){
                if(data.error === "NameExists"){
                    console.log('An error has occured while creating source! Source with this name already exists.');
                    $("#addSourceFooter").append("<p class='text-danger' id=\"add-error\">Source with this name already exists.</p>");
                }
                else {
                    console.log('An error has occured while creating source!');
                    $("#addSourceFooter").append("<p class='text-danger' id=\"add-error\">An error has occured while adding source!</p>");
                }
                return;
            }
            
            
            
            console.log('Source added!');
            showToast("createSourceToast");
            loadSources();
            $('#addSourceModal').modal('toggle');
            $("#addSourceForm").trigger("reset");
        },
        error: function() {
            console.log('Error adding source!');
            $("#addSourceFooter").append("<p class='text-danger' id=\"add-error\">An error has occured while adding source!</p>");
        }
    });
}

function saveEditedModal() {
    let form = $("#editSourceForm");
    
    const data = form.serialize();
    $.ajax({
        url: "/Api/editSource/",
        method: "POST",
        data: data,
        success: function(data) {
            if(data.error != null){
                console.log('An error has occured while editing source!');
                $("#editSourceFooter").append("<p class='text-danger' id=\"edit-error\">An error has occured while editing source!</p>");
                
                return;
            }
            
            
            console.log('Source edited!');
            showToast("editSourceToast");
            loadSources();
            $('#editSourceModal').modal('toggle');
            $("#editSourceForm").trigger("reset");
        },
        error: function() {
            console.log('An error has occured while editing source!');
            $("#editSourceFooter").append("<p class='text-danger' id=\"edit-error\">An error has occured while editing source!</p>");
        }
    });
}

function loadSources(){
    $.ajax({
        url: "/Api/getSources/",
        method: "GET",
        dataType: "json",
        success: function(data) {
            sources = data;
            console.log('Sources downloaded!');


            let tableBody = $("#sources tbody");
            tableBody.empty();
            sources.forEach(function(source) {

                const row = `
                    <tr>
                        <td> ${source.name} </td>
                        <td> ${source.type} </td>
                        <td> ${source.cacheTime} </td>
                        <td> ${source.errorRow} </td>
                        <td>
                        <button class="btn btn-dark rounded-3 text-white border-0 px-3 py-1" onclick='editSource("${source.name}")'> Edit <i class="bi bi-pencil"></i></button>
                        <button class="btn btn-danger rounded-3 text-white border-0 px-3 py-1" onclick='removeSource("${source.name}")'> Remove <i class="bi bi-trash"></i> </button> </td>
                    </tr>
                `;

                tableBody.append(row);
            });
            console.log("Sources loaded!");
        },
    });
}

function loadSourcesList() {
    $.ajax({
        url: "/Api/getSources/",
        method: "GET",
        dataType: "json",
        success: function(data) {
            sources = data;
            console.log('Sources downloaded!');


            let sourcesList = $("#sourcesList");

            sourcesList.empty();
            sources.forEach(function(source) {
                sourcesList.append(`
                <div class="border border-dark d-flex flex-row justify-content-between align-items-center rounded-2 shadow-sm">
                    <p class="text-center m-0 mx-1">
                        ${source.name}
                    </p>
                    <button class="btn btn-dark" onclick="downloadAndLoad('${source.name}')">
                        <i class="bi bi-cloud-download"></i>
                        Load
                    </button>
                </div>
                `);
            });
        },
    });
}

function removeSource(sourceName){
    $.ajax({
        url: "/Api/removeSource/" + sourceName,
        method: "DELETE",
        success: function(data) {
            
            if(data.error != null) {
                let error = data.error;
                
                if(error === "NotAdmin"){
                    showToast("accessDenitedToast");
                }
                
                return;
            }
            
            console.log('Source removed!');
            showToast("removeSourceToast");
            loadSources();
        },
        error: function() {
            console.log('Error removing source!');
        }
    });
}

function editSource(sourceName){
    console.log('Editing source: ' + sourceName);
    
    let modal = $("#editSourceModal");
    let form = $("#editSourceForm");
    
    
    if(isAdmin === false){
        showToast("accessDenitedToast"); return;
    }

    let source = sources.find(source => source.name === sourceName);
    
    if(source == null){
        console.log('Source not found!');
        showToast("sourceNotFoundToast");
        return;
    }
    
    form.find("#name-editS").val(sourceName);
    form.find("#connectionString-editS").val(source.connectionString);
    
    form.find("#cacheTime-editS").val(source.cacheTime);
    form.find("#errorRow-editS").val(source.errorRow);
    
    modal.modal("toggle")
}