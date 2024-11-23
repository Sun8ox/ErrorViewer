let data = []
let dataFiltered = []

function downloadData(sourceName){
    
    $.ajax({
        url: "/Api/getDataFromSource/" + sourceName,
        type: "GET",
        success: function(response){
            data = response
            console.log("Data downloaded!")
        },
        error: function (response) {
            console.log(response);
            showToast("getDataFailed")
        }
        
    });
}


function searchInData() {
    
    let searchInput = $("#searchInput").val();
    
    dataFiltered = data.filter(function(row) {
        return row.some(function(column){
            return column.toString().includes(searchInput);
        });
    });
    
    loadDataFiltered();
}

function resetSearch(){
    dataFiltered = []
    loadData();
}

function loadDataFiltered() {

    let table = $("#dataTable");
    
    let tableBody = table.find("tbody");
    
    tableBody.empty();

    if(dataFiltered.length > 0) {
        dataFiltered.forEach(function (row) {
            if(row === dataFiltered[0]) return;

            tableBody.append("<tr>")
            row.forEach(function (column){
                tableBody.append(`
                <td>${column}</td>
            `);
            });
            tableBody.append("</tr>")
        });
    }

    console.log("Data filtered!")
}

function loadData() {
    
    let table = $("#dataTable");
    
    let tableHead = table.find("thead");
    let tableBody = table.find("tbody");
    
    tableHead.empty();
    tableBody.empty();
    
    
    if(data.length > 0) {
        tableHead.append("<tr>");
        data[0].forEach(function (column) {
            tableHead.append(`<th>${column}</th>`);
        });
        tableHead.append("</tr>")

        data.forEach(function (row) {
            if(row === data[0]) return;

            tableBody.append("<tr>")
            row.forEach(function (column){
                tableBody.append(`
                <td>${column}</td>
            `);
            });
            tableBody.append("</tr>")
        });
    }
    
    console.log("Data loaded!")
}

function downloadAndLoad(sourceName){
    $.ajax({
        url: "/Api/getDataFromSource/" + sourceName,
        type: "GET",
        success: function(response){
            data = response
            console.log("Data downloaded!")

            loadData();
        },
        error: function (response) {
            console.log(response);
            showToast("getDataFailed")
        }

    })
}