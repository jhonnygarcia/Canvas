var pageData = {
};
$(document).ready(function () {
    $("#txt-edad,#txt-edad1").TouchSpin({
        verticalbuttons: true
    });
    $('#txt-time').timepicker({ defaultTime: false });
    var tabla = $('#example').DataTable({
        searching: false,
        responsive: true,
        serverSide : true,
        ajax: function (data, callback, settings) {
            var params = {};
            params.PageIndex = data.start;
            params.ItemsPerPage = data.length;
            params.OrderDirection = data.order[0].dir;
            params.OrderColumnName = decode(
                data.order[0].column,
                [
                    0, 'id',
                    1, 'nombre',
                    2, 'direccion',
                    3, 'salario',
                    4, 'fecha-nacimiento',
                    5, 'pais'
                ]);
            params.Id = 4;
            params.Name = 'XHR';
            $.ajax({
                url: appData.siteUrl + 'api/v1/clientes/advanced-search',
                type: 'POST',
                contentType: 'application/json',
                data: JSON.stringify(params),
                success: function (data, status, xhr) {
                    var rows = [];
                    $.each(data.Data, function(index, value) {
                        var row = [];
                        row.push(value.Id);
                        row.push(value.Nombre);
                        row.push(value.Direccion);
                        row.push(value.Salario);
                        row.push(value.Pais);
                        row.push(value.FechaNacimiento);
                        rows.push(row);
                    });

                    callback({
                        "draw": 1,
                        "recordsTotal": 10,
                        "recordsFiltered": data.TotalElements,
                        "data": rows
                    });
                }
            });
            //console.log(data, callback, settings);
        }
        //ajax: {
        //    url: '/my/path',
        //    data: function (data, callback, settings) {

        //    }
        //}
    });
    new $.fn.dataTable.FixedHeader(tabla);

    $('#cbx-pais').jquick({
        url: 'https://erpacademico.unir.net/api/v1/estudios',
        changeParams: {
            SearchText: 'nombre'
        },
        successData: {
            Description: 'Nombre'
        },
        toolbar: {
            fnedit: function () {
                console.log('fnEdit');
            },
            fnadd: function () {
                console.log('fnadd');
            },
            fncopy: function () {
                console.log('fncopy');
            },
            fnsearch: function () {
                console.log('fnsearch');
            }
        }
    });
});