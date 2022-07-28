// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

//const { data } = require("jquery");

// Write your JavaScript code.

if (performance.navigation.type == performance.navigation.TYPE_RELOAD) {
    window.sessionStorage.setItem("createShown", 'f');
    window.sessionStorage.setItem("editShown", 'f');
}

let qrVid = document.getElementById('preview');
let qrClose = document.getElementById('close-qr');
let scanner = new Instascan.Scanner({ video: qrVid });


$('#c-submit').on('click', function () {
    var check = document.getElementsByClassName("c-null-check");

    for (const i of check) {
        var res = true;
        if (!i.value) {
            i.nextElementSibling.innerHTML = "This field is required";
            res = false;
        }
    }
    if(res)
        $('#create-form').submit();
});

function scanQr() {
    qrVid.style.display = "initial";
    qrClose.style.display = "initial";
    scanner.addListener('scan', function (content) {
        console.log(content);
        $.ajax({
            type:'Post',
            url: 'ActivateEmployee',
            data: { id: content },
            success: function (response) {
                window.location.href = response.redirectToUrl;
            }
        });
   });
    Instascan.Camera.getCameras().then(function (cameras) {
        if (cameras.length > 0) {
            scanner.start(cameras[0]);
        } else {
            console.error('No cameras found.');
        }
    }).catch(function (e) {
        console.error(e);
    });
}

function closeQr() {
    scanner.stop();
    qrVid.style.display = "none";
    qrClose.style.display = "none";
}


var createShown = false;//window.sessionStorage.getItem("createShown") == 't';
var createModal = document.getElementById("c-modal");

var editShown = false; // window.sessionStorage.getItem("editShown") == 't';
var editModal = document.getElementById("e-modal");

var deleteShown = false;
var lastId = "";

var lastEmpId = "";

var employeesShown = false;
var empModal = document.getElementById("emp-modal");

if (createShown) {
    createModal.style.display = "initial";
}

if (editShown) {
    editModal.style.display = "initial";
}

function Create() {
    if (!createShown) {
        createModal.style.display = "initial";
    }
    else {
        createModal.style.display = "none";
    }
    createShown = !createShown;
    window.sessionStorage.setItem("createShown", createShown ? 't' : 'f')
}

function Delete(id) {
    lastId = id;
    if (!deleteShown) {
        document.getElementById(id).style.display = "initial";
    }
    else {
        document.getElementById(id).style.display = "none";
    }
    deleteShown = !deleteShown;
}

function DeleteEmp(id) {
    lastEmpId = id;
    if (!deleteShown) {
        document.getElementById(id).style.display = "initial";
    }
    else {
        document.getElementById(id).style.display = "none";
    }
    deleteShown = !deleteShown;
}


function Edit(id) {

    if (!editShown) {
        editModal.style.display = "initial";
        $.ajax({
            type: 'GET',
            url: 'Departments/getDepartment?id=' + id,
            contentType: 'json',
            success: function (result) {
                //console.log('Data received: ');
                //console.log(result);
                document.getElementById("edit-name-input").value = result.name;
                document.getElementById("edit-limit-input").value = result.limit;
                document.getElementById("edit-description-input").value = result.description;
                document.getElementById("edit-id").value = result.id;
            }
        });
    }
    else {
        editModal.style.display = "none";
    }
    editShown = !editShown;
    window.sessionStorage.setItem("editShown", editShown ? 't' : 'f');
}

function EditEmp(id) {

    if (!editShown) {
        editModal.style.display = "initial";
        $.ajax({
            type: 'GET',
            url: 'User/getEmployee?id=' + id,
            contentType: 'json',
            success: function (result) {
                //console.log('Data received: ');
                console.log(result);
                document.getElementById("edit-name-input").value = result.name;
                document.getElementById("edit-dateAdded-input").value = result.dateAdded;
                document.getElementById("edit-id").value = result.id;
                document.getElementById("edit-pp").src = "uploads/".concat(result.profilePicture ? result.profilePicture : "default.jpg");
                document.getElementById("user-qr").src = "https://chart.googleapis.com/chart?chs=150x150&cht=qr&chl=".concat(result.id);

                $.ajax({
                    type: 'GET',
                    url: 'Departments/getDepartment?id=' + result.departmentId,
                    contentType: 'json',
                    success: function (res) {
                        //console.log('Data received: ');
                        //console.log(result);
                        document.getElementById("edit-depName-input").value = res.name;

                    }
                });

            }
        });
    }
    else {
        editModal.style.display = "none";
    }
    editShown = !editShown;
    window.sessionStorage.setItem("editShown", editShown ? 't' : 'f');
}

function Departments() {

    $.ajax({
        type: 'GET',
        url: 'User/getDepartments',
        contentType: 'json',
        success: function (result) {
            //console.log('Data received: ');
            console.log(result);
            var sel = document.getElementById("emp-select-dep");
            var sele = document.getElementById('edit-depName-input');
            //console.log(sel);
            //console.log(sele);
            for (var i in result) {
                var op = document.createElement("option");
                op.text = result[i].name;

                var op1 = document.createElement("option");
                op1.text = result[i].name;
                sele.add(op);
                sel.add(op1);
            }
        }
    });
}

function Employees(id) {
    if (!employeesShown) {
        empModal.style.display = "initial";
        $.ajax({
            type: 'GET',
            url: 'Departments/getEmployees?id=' + id,
            contentType: 'json',
            success: function (result) {
                //console.log('Data received: ');
                //console.log(result);
                var table = "";
                for (var emp in result) {
                    //console.log(result[emp]);
                    table += "<tr> \n <td> \n";
                    table += result[emp].name + '\n';
                    table += "<td> \n";
                    table += result[emp].dateAdded;
                    //table += "<\\td> \n <\\tr>";
                }
                //console.log(table);
                document.getElementById("emp-table-body").innerHTML = table;

            }
        });
    }
    else {
        empModal.style.display = "none";
    }
    employeesShown = !employeesShown;
}

function clss() {
    sessionStorage.clear();
}

window.onclick = function (event) {
    if (event.target == createModal || event.target == editModal || event.target == document.getElementById(lastId) || event.target == document.getElementById(lastEmpId) || event.target == empModal) {
        if (createModal) {
            createModal.style.display = "none";
            createShown = false;
            window.sessionStorage.setItem("createShown", 'f');
            document.getElementById("create-form").reset();
        }

        if (editModal) {
            editModal.style.display = "none";
            editShown = false;
            window.sessionStorage.setItem("editShown", 'f');
            document.getElementById("edit-form").reset();
        }

        if (lastId != "") {
            document.getElementById(lastId).style.display = "none";
            deleteShown = false;
        }

        if (lastEmpId != "") {
            document.getElementById(lastEmpId).style.display = "none";
            deleteShown = false;
        }

        if (empModal) {
            empModal.style.display = "none";
            employeesShown = false;
        }
    }
}


