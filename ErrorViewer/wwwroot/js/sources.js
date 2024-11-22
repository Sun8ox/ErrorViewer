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
                    console.log('Error loading sources!');
                    $("#addSourceFooter").append("<p class='text-danger' id=\"add-error\">Source with this name already exists.</p>");
                }
                else {
                    console.log('Error loading sources!');
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
    console.log("Not implemented yet!");
    showToast("notImplementedToast");
}