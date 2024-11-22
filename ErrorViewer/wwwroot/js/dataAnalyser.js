let errorTypes = []
let errorTypesFiltered = []

function countErrorsAtRow(columnName){
    
    let errorTypes = []
    let alreadySeenErrors = []
    
    let rowNumber = 0;
    
    // for row in data: (python)
    // foreach(var row in data){} (C#)
    // for (var row : data){} (Java)
    // Javascript:
    
    data[0].forEach(function (column) {
        if(column === columnName){
            rowNumber = data[0].indexOf(column);
            return true;
        }
    });
    
    data.forEach(function (row){ 
        col = row[rowNumber];
        
        if(row[0] === data[0][0]) return;
        
        if(alreadySeenErrors.includes(col)){
            errorTypes.forEach(function (errorType){
               if(errorType[0] === col) {
                       errorType[1] += 1;
                       return true;
               }
               return false;
            });
        } else {
            alreadySeenErrors.push(col);
            errorTypes.push([col, 1]);
        }
    });
    return errorTypes;
}

function showErrorCounts(columnName){
    let table = $("#errorCountsTable tbody");
    let errorCountList = $("#errorCounts");
    
    table.empty();
    errorCountList.empty();
    
    
    
    let colors = ["btn-primary", "btn-secondary", "btn-success", "btn-danger", "btn-warning", "btn-info", "btn-light", "btn-dark"];
    
    data[0].forEach(function (column) {
        errorTypes = countErrorsAtRow(columnName);
        
        let i = 0;
        let iMax = colors.length-1;
        
        errorTypes.forEach(function (errorType) {

            if(errorType[1] > 100) {
                errorCountList.append(`

                <div class="btn ${colors[i]} w-auto h-auto d-flex flex-column gap-2">
                    <h2>${errorType[1]}x</h2>
                    <h6 style="width: 10rem; white-space: wrap">${errorType[0]}</h6>
                </div>
   
            `);
            }
            
            table.append(
                `
                <tr>
                    <td>${errorType[0]}</td>
                    <td>${errorType[1]}</td>
                </tr>
                `
            );
            
            i++;
            if(i > iMax) i = 0;
        });
    });
    
}


function loadSourcesList2() {
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
                    <p class="text-center m-0 mx-3">
                        ${source.name}
                    </p>
                    <button class="btn btn-dark" onclick="downloadAndLoad2('${source.name}', '${source.errorRow}')">
                        Load
                    </button>
                </div>
                `);
            });
        },
    });
}

function downloadAndLoad2(sourceName, errorRow){
    $.ajax({
        url: "/Api/getDataFromSource/" + sourceName,
        type: "GET",
        success: function(response){
            data = response
            console.log("Data downloaded!")

            showErrorCounts(errorRow);
        },
        error: function (response) {
            console.log(response);
            showToast("getDataFailed")
        }

    })
}

function searchInData2() {

    let searchInput = $("#searchInput2").val();

    errorTypesFiltered = errorTypes.filter(function(errorType) {
        return errorType[0].includes(searchInput.toLowerCase());
    });

    let table = $("#errorCountsTable tbody");
    table.empty();

    errorTypesFiltered.forEach(function (errorType) {
        table.append(
            `
                <tr>
                    <td>${errorType[0]}</td>
                    <td>${errorType[1]}</td>
                </tr>
                `
        );
    });
    
}

function resetSearch2(){
    let table = $("#errorCountsTable tbody");
    table.empty();

    errorTypes.forEach(function (errorType) {
        table.append(
            `
                <tr>
                    <td>${errorType[0]}</td>
                    <td>${errorType[1]}</td>
                </tr>
                `
        );
    });
}