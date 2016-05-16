function doinitialize(parent) {
    $(parent + ' .date').datepicker({ showOn: 'button' });
    $(parent + ' .autocomplete').each(function (i, el) {
        addbuttonautocomplete(el);
    });

    $(parent + ' .fecha').datepicker({ showOn: 'button', buttonImage: "/content/images/calendar.gif" })
               .blur(function (e) {
                   $(this).val(formatFecha($(this).val(), $(this).hasClass("futuredatetime")));
               });


    $(parent + ' .monthpicker').monthpicker({
        'monthNames': ['Ene', 'Feb', 'Mar', 'Abr', 'May', 'Jun', 'Jul', 'Ago', 'Sep', 'Oct', 'Nov', 'Dic'],
        openOnFocus: false
    }).blur(function (e) {
        $(this).val(formatMonth($(this).val()));
    });
    $(parent + ' .comprobante').blur(function (e) {
        $(this).val(formatComprobante($(this).val()));
    });

    $('<img />')
        .attr('src', '/content/images/calendar.gif')
		.attr('tabIndex', -1)
        .attr('class', 'ui-monthpicker-trigger')
		.attr('title', 'Calendario de meses')
		.insertAfter($(parent + ' .monthpicker'))
		.click(function () {
		    $(this).prev().monthpicker('show');
		});

    $(parent + " .spinner").spinner({ min: 0, max: 10000000 });
    $(parent + ' .timeago').timeago();
    $(parent + ' .ui-datepicker-trigger').attr("tabindex", "-1");
    $(parent + " .decimales").calculadora({ decimals: 2, useCommaAsDecimalMark: true })
        .numpadDecSeparator({ separator: "," })
        .numeric(",")
        .focus(function () { if ($(this).val() == '0,00' || $(this).val() == '0') $(this).val(''); });
    $(parent + " .entero").calculadora({ decimals: 0, useCommaAsDecimalMark: true })
        .numeric()
        .focus(function () { if ($(this).val() == '0') $(this).val(''); });
    $(parent + " .solonumeros").numeric();
    $(parent + ' .superlistinput').each(function (i, el) {
        dosuperlistinput(el);
    });

    $(parent + ' .itemlistinput').each(function (i, el) {
        doitemlistinput(el);
    });

    $(parent + ' .productolistinput').each(function (i, el) {
        dosuperlistinput(el, function (elControl, data) {
            var row = elControl.closest("tr");
            row.find("input").each(function (i, elInput) {
                elInput = $(elInput);
                if (elInput.attr("name").endsWith("PesoUnitario") && data.PesoUnitario != null && data.PesoUnitario > 0) elInput.val(data.PesoUnitario).change();
                if (elInput.attr("name").endsWith("PrecioUnitario") && data.PrecioUnitario != null && data.PrecioUnitario > 0) elInput.val(data.PrecioUnitario).change();
                if (elInput.attr("name").endsWith("PrecioKg") && data.PrecioKg != null && data.PrecioKg > 0) elInput.val(data.PrecioKg).change();
                if (elInput.attr("name").endsWith("ProveedorID") && data.ProveedorID != null && data.ProveedorID > 0) elInput.val(data.ProveedorID).change();
            });
            }
            , function (elControl) { // OnFailture: si falla que lleve todos los valores de la linea a vacio.
                var row = elControl.closest("tr");
                row.find("input").each(function (i, elInput) {
                    if (!$(this).attr("name").endsWith("ItemMercaderiaID"))
                        $(this).val("").change();
                });
            });            
        });

    $(parent + ' .usuariolistinput').each(function (i, el) {
        var onSuccess = null;
        if ($(this).data('params')['Rol'] == 'Proveedor') {
            // si se selecciona el proveedor, entonces altero los controles para que solo busquen productos de este proveedor.
            onSuccess= function (elControl, data) {
                $(".productolistinput").each(function (i, el) {
                    $(this).data('params')['ProveedorID'] = data.ID;
                    $(this).attr('data-params', '{"ProveedorID":"' + data.ID + '"}');
                });
            }
        }
        dosuperlistinput(el,onSuccess);
    });

    $(parent + ' .peso').each(function (i, el) {
        $(el).change(function () {
            // cambio en la sumatoria
            var sumatoriaPeso = 0;
            $(".peso").each(function (i, control) {
                var pesoASumar = parseFloat($(control).val().replace(".", "").replace(",", "."));
                if (pesoASumar > 0) sumatoriaPeso += pesoASumar;
            });
            $(".pesototal").each(function (i, control) {
                $(control).val(Math.round(sumatoriaPeso, 2)).change();
            });

            // cambio en el precio
            var row = $(this).closest('tr')
            var peso = parseFloat($(el).val().replace(".", "").replace(",", "."));
            var precioKg = 0;
            row.find('.preciokg').each(function (i, el) {
                precioKg = parseFloat($(this).val());
            });

            if (peso > 0 && precioKg != null && precioKg > 0) {
                row.find('.precio').each(function (i, control) {
                    row.find('.precio').each(function (i, control) {
                        var precio = parseFloat($(control).val().replace(".", "").replace(",", "."));
                        precio = precioKg * peso;
                        $(control).val(precio).change();
                    });
                });
            }
        });
    });

    $(parent + ' .bultos').each(function (i, el) {
        $(el).change(function () {
            var sumatoriabultos = 0;
            $(".bultos").each(function (i, control) {
                var bultosASumar = parseInt($(control).val());
                if (bultosASumar > 0) sumatoriabultos += bultosASumar;
            });
            $(".bultostotal").each(function (i, control) {
                $(control).val(sumatoriabultos).change();
            });
        });
    });

    $(parent + ' .precio').each(function (i, el) {
        $(el).change(function () {
            var sumatoriaprecio = 0;
            $(".precio").each(function (i, control) {
                var precioASumar = parseFloat($(control).val().replace(".","").replace(",", "."));
                if (precioASumar > 0) sumatoriaprecio += precioASumar;
            });
            $(".preciototal").each(function (i, control) {
                $(control).val(Math.round(sumatoriaprecio, 2)).change();
            });
        });
    });

    $(parent + ' .pesounitario').each(function (i, el) {
        $(el).change(function () {
            var row = $(this).closest('tr');
            var cantidad = 0;
            var pesounitario = parseFloat($(el).val().replace(".", "").replace(",", "."));
            row.find('.cantidad').each(function (i, el) {
                cantidad = parseInt($(this).val());
            });
            if (cantidad > 0 && pesounitario > 0) {
                row.find('.peso').each(function (i, control) {
                    var peso = parseFloat($(control).val().replace(".", "").replace(",", "."));
                    peso = pesounitario * cantidad;
                    $(control).val(peso).change();
                });
            }
        });
    });
    $(parent + ' .preciounitario').each(function (i, el) {
        $(el).change(function () {
            var row = $(this).closest('tr');
            var cantidad = 0;
            var preciounitario = parseFloat($(el).val());
            row.find('.cantidad').each(function (i, el) {
                cantidad = parseInt($(this).val());
            });
            if (cantidad > 0 && preciounitario != null && preciounitario > 0) {
                row.find('.precio').each(function (i, control) {
                    var precio = parseFloat($(control).val().replace(".", "").replace(",", "."));
                    precio = preciounitario * cantidad;
                    $(control).val(precio).change();
                });
            }
        });
    });

    $(parent + ' .cantidad').each(function (i, el) {
        $(el).change(function () {
            var row = $(this).closest('tr')
            var cantidad = parseInt($(el).val());
            var pesounitario = 0;
            row.find('.pesounitario').each(function (i, el) {
                pesounitario = parseFloat($(this).val().replace(".", "").replace(",", "."));
            });
            var preciounitario = 0;
            row.find('.preciounitario').each(function (i, el) {
                preciounitario = parseFloat($(this).val().replace(".", "").replace(",", "."));
            });

            if (cantidad > 0 && preciounitario != null && preciounitario > 0) {
                row.find('.precio').each(function (i, control) {
                    row.find('.precio').each(function (i, control) {
                        var precio = parseFloat($(control).val().replace(".", "").replace(",", "."));
                        precio = preciounitario * cantidad;
                        $(control).val(precio).change();
                    });
                });
            }
            if (cantidad > 0 && pesounitario > 0) {
                row.find('.peso').each(function (i, control) {
                    row.find('.peso').each(function (i, control) {
                        var peso = parseFloat($(control).val().replace(".", "").replace(",", "."));
                        peso = pesounitario * cantidad;
                        $(control).val(peso).change();
                    });
                });
            }
        });
    });


    $(parent + ' .preciokg').each(function (i, el) {
        $(el).change(function () {
            var row = $(this).closest('tr');
            var peso = 0;
            var preciokg = parseFloat($(el).val().replace(".", "").replace(",", "."));
            row.find('.peso').each(function (i, el) {
                peso = parseFloat($(this).val().replace(".", "").replace(",", "."));
            });
            if (peso > 0 && preciokg > 0) {
                row.find('.precio').each(function (i, control) {
                    var precio = parseFloat($(control).val().replace(".", "").replace(",", "."));
                    precio = preciokg * peso;
                    $(control).val(precio).change();
                });
            }
        });
    });
    

    $(parent + ' .spinnerPlus').each(function (i, el) {
        $(this).click(function(){
            var control = $("#" +$(this).attr('id').replace("_spinnerPlus", ""));
            if (control) {
                var value =  control.val();
                if (value == "" || value == null) {
                    $(control).val('1').change();
                }
                else {
                    var valorControlDecrementado = parseFloat(value.replace(".", "").replace(",", ".")) + 1;
                    $(control).val(valorControlDecrementado.toString().replace(".", ",")).change();
                }
            }
        });
    });

    $(parent + ' .spinnerMinus').each(function (i, el) {
        $(this).click(function () {
            var control = $("#" + $(this).attr('id').replace("_spinnerMinus", ""));
            if (control) {
                var value = control.val();
                if (value == "" || value == null) {
                    $(control).val('0').change();
                }
                else {                    
                    var valorControlDecrementado = parseFloat(value.replace(".", "").replace(",", ".")) - 1;
                    if (valorControlDecrementado < 0)
                        $(control).val('0').change();
                    else
                        $(control).val(valorControlDecrementado.toString().replace(".", ",")).change();
                }
            }
        });
    });

    $(parent + ' .simpleautocomplete').each(function (i, el) {
        doSimpleAutocomplete(el);
    });


    $(parent + ' .addbuttonautocomplete').each(function (i, el) {
        addbuttonautocomplete(el);
    });


    $('<span><i class="icon-cancel" style="color: crimson" /></span>')
		.attr('tabIndex', -1)
		.attr('title', 'Resetear filtro')
		.insertAfter($(parent + ' input.quicksearch'))
		.click(function () {
		    $("table.filtrable tbody tr").show();
		    $("input.quicksearch").val("").hide();
		})
        .hide();

    $(parent + ' .filtrablerow').click(function () {
        $("input.quicksearch").show().focus().next().show();
    });
    $(parent + ' input.quicksearch').focusout(function () { if ($("input.quicksearch").val() == "") { $("input.quicksearch").hide().next().hide() } })
                          .quicksearch('table.filtrable tbody tr');
    $(parent + ' input.simplefilter').quicksearch('table.filtrable tbody tr');
    $(parent + " .sortable").tablesorter({ dateFormat: 'uk' });
    $(parent + " .nicetable thead tr:first").addClass("ui-widget-header");
    //$(parent + " .filtrable").not('.nosticky').stickyTableHeaders();
    $(parent + " .help").addClass("ui-icon-help").addClass("ui-icon");
    $(parent + " input[title]").autohelp("#autohelp");
    $(parent + " :input[readonly]").attr("tabindex", "-1");

    $(parent + ' .hora').blur(function (e) {
        $(this).val(formatHora($(this).val()));
    }).attr("placeholder", "00:00");


    $(parent + ' .field-validation-error').each(function () {
        $(this).attr('title', $(this).html());
        $(this).html("<i class='icon-cancel' style='color: red; font-size: 16px' />");
    });

    $(parent + ' .tip').webuiPopover({
        trigger: 'hover'
    });

    $(parent + ' .minimenu').webuiPopover({
        trigger: 'click',
		placement: 'bottom'
    });

    $(parent + ' .masinfo').each(function () {
        var url = $(this).attr('href');
        var title = $(this).attr('alt');
        var width = $(this).data('width') || "auto";
        var height = $(this).data('height') || "auto";
        $(this).webuiPopover({
            type:'async',
            url: url,
            width: width,
            height: height,
            closeable: true,
            title: title,
            cache: false,

            content: function (data) {
                return data;
            }
        });
    });



    $(parent).contextmenu({
        delegate: ".nicetable .header",
        menu: [
            { title: "Bajar esta tabla", action: function(event, ui){
                generateExcel(ui.target.closest("table"));
                }
            }
        ]
    });


    $(parent + " .agregarLinea").each(function () {
        var self = $(this);
        var table = self.closest('table');
        var senseInputSufix = self.data("senseinputsufix");
        if (senseInputSufix != "") {
            //table.find('input[id$="__' + senseInputSufix + '"]').blur(function () {
            table.on('blur', 'input[id$="__' + senseInputSufix + '"]', function () {
                // me fijo si hay suficientes vacíos
                var CantidadDeLineasVacias = 0;
                table.find('input[id$="__' + senseInputSufix + '"]').each(function () {
                    if ($(this).val() == "" || $(this).val() == 0) {
                        CantidadDeLineasVacias++;
                    }
                });
                if (CantidadDeLineasVacias <= 2) {
                    // tengo que agregar para tenga espacio de agregar
                    AgregarLinea(table);
                }
            });
        }

        // le agrego la parte manual tambien
        self.click(function () { AgregarLinea(table); });
    });


    $(parent + " table input, table " + parent + " input").keyup(function (event) {
        if (event.which == 40 || event.which == 38) { // flecha abajo y arriba
            self = $(this);
            if (self.hasClass('ui-autocomplete-input')) return false;
            var id = self.attr("id");
            var firstPoint = id.indexOf("_");
            var lastPoint = id.indexOf("__");
            if (firstPoint > 0 && lastPoint > 0 && firstPoint < lastPoint) {
                // me aseguro que sea una tabla de edición
                var tr = self.closest('tr');
                var nextTr = event.which == 40 ? tr.next() : tr.prev();
                var nextEndID = id.substring(lastPoint);
                var targetel = nextTr.find("input[id$='" + nextEndID + "']");
                if (targetel.length > 0) {
                    targetel.focus();
                }
            }
        }
    });


    $(parent + " #SeleccionarChequeDialog").dialog({
        autoOpen: false, width: 600, height: 400, modal: true,
        buttons: {
            "Cancelar": function () { $(this).dialog("close"); }
        }
    });

    $('<img />')
        .attr('src', '/content/images/cheque.png')
		.attr('tabIndex', -1)
		.attr('title', 'Seleccionar cheque')
        .attr('class', 'SeleccionarChequeImg')
		.insertAfter($(parent + ' .SeleccionarCheque'))
		.click(function () {
		    var self = $(this);
		    var id = self.prev().attr("id");
		    $("#SeleccionarChequeDialog").html("")
                .dialog("option", "title", "Seleccionar cheque de valores en cartera")
                .load("/Asiento/SeleccionarCheque/" + id, function () { $("#SeleccionarChequeDialog").dialog("open"); });
		});





    // Le saco la clase al avoidflicker para que se muestra nuevamente
    $(".avoidflicker").removeClass('avoidflicker');

    // Le saco la clase para que quede lindo despues de un momento
    $(".hightlightforamoment").delay(2000).removeClass('hightlightforamoment');

    // Le saco el flag de rebind
    $(".newelements").removeClass('newelements');


}


$(function () {
    doinitialize('body');

    $('.minidelete, .minilivedelete, .jbtn-borrar, .delete').live("click", function () {
        event.preventDefault();
        var self = $(this);
        var url = self.attr("href");
        $("#minidelete-dialog").dialog({
            autoOpen: true, width: 'auto', height: 'auto', modal: true,
            buttons: [{
                text: self.data("buttontext") || "Borrar",
                click: function () {
                    $.post(url, function (data) {  //Post to action
                        if (data == 'OK') {
                            if (self.data("callback")) {
                                eval(self.data("callback"));
                            }
                            else {
                                if (self.closest('li').length) {
                                    self.closest('li').remove();
                                } else { // must be in a table
                                    self.closest('tr').remove();
                                    $(".totales").html("<td colspan='20' class='smalltext'>Refresque la consulta para ver el total</td>");
                                }
                            }
                        }
                        else {
                            ShowError(data);
                        }
                    });
                    $(this).dialog("close");
                }
            }, {
                text: "Cancelar",
                click: function () {
                    $(this).dialog("close");
                }
            }]
        });
    });
    $('.minipost').live("click", function () {
        event.preventDefault();
        var self = $(this);
        var url = self.attr("href");
        $.post(url, function (data) {  //Post to action
            if (data == 'OK') {
                if (self.data("callback")) {
                    eval(self.data("callback"))
                }
            }
            else {
                ShowError(data);
            }
        });
    });

    //postea en next form
    $('.miniexcel').live("click", function () {
        event.preventDefault();
        var self = $(this);
        var url = self.attr("href");
        var formselector = self.data("formselector");
        var elform;
        if (!formselector || formselector == "") {
            elform = $('body').find('form').first();
        }
        else {
            elform = $(formselector);
        }
        var prevaction = elform.attr('action');
        elform.attr('action', url);
        elform.submit();
        elform.attr('action', prevaction);
        $.unblockUI();
        $('#loadingDiv').hide();
    });
    $(".miniedit").live("click", function (event) {
        event.preventDefault();
        var self = $(this);
        var url = self.attr("href");
        var elDialog = $('<div />').appendTo('body');
        //var elDialog = $('#miniedit-dialog');
        elDialog.load(url, function () {
            elDialog.dialog({
                autoOpen: true, width: 'auto', height: 'auto', modal: true,
                open: function (event, ui) { if (self.data("checkcanupdate")) CanUpdate(); },
                close: function (event, ui) {
                    //elDialog.dialog("close") // 
                    elDialog.dialog('destroy').remove();
                },
                buttons: [{
                    id: "buttonGuardar",
                    text: self.data("buttontext") || "Guardar",
                    click: function () {
                        if (self.data("beforepost") != "") eval(self.data("beforepost"));
                        $.post(url.toLowerCase().replace("nuevo", "editar"),
                            elDialog.find("form").serialize(),
                            function (data) {
                                if (data == 'OK') {
                                    //elDialog.dialog("close"); // 
                                    elDialog.dialog('destroy').remove();
                                    if (self.data("callback")) eval(self.data("callback"));
                                }
                                else if (data.startsWith('OKURL:')) {
                                    elDialog.dialog('destroy').remove();
                                    window.location = data.substring(6);
                                }
                                else {
                                    ShowError(data);
                                }
                            });
                    }
                }
                ]
            });
        });
    });

    

    $(".minidialog").live("click", function (event) {
        event.preventDefault();
        var url = $(this).attr("href");
        var elDialog = $('<div />').appendTo('body');
        elDialog.load(url, function () {
            elDialog.dialog({
                autoOpen: true, width: 'auto', height: 'auto', modal: true,
                close: function (event, ui) {
                    elDialog.dialog('destroy').remove();
                }
            })
        });
    });

    $(".miniprint").live("click", function (event) {
        event.preventDefault();
        var self = $(this);
        var url = self.attr("href");
        $("#printjob").remove();
        $('<iframe id="printjob" src="' + url + '">')
        .load(function () {
            this.focus();
            document.getElementById("printjob").contentWindow.print();
        })
        .css({ position: "absolute", left: "-10000px", top: "0px" })
        .appendTo(document.body);
    });

    $(".minisearch").live("click", function (event) {
        event.preventDefault();
        var self = $(this);
        var url = self.attr("href");
        var where = self.data("where");
        $("#minisearch").remove();
        var box = $("<div id='minisearch' title='Buscar'><input type='search' name='q' autocomplete='off' placeholder='" + where + "' /><div class='results'></div></div>")
            .appendTo(document.body);

        var delayTime = 500;
        var lastTimeout = null;

        var input = $("#minisearch input");
        var suggest = $("#minisearch .results");

        input.keyup(function (e) {
            switch (e.keyCode) {
                //Other keys. 
                default:
                    if (lastTimeout != null) {
                        clearTimeout(lastTimeout);
                    }
                    var value = this.value;
                    if (value.length == 0) {
                        suggest.hide();
                    }
                    else {
                        lastTimeout = window.setTimeout(function () {
                            queryServer(value);
                        }, delayTime);
                    }
            }
        });

        function queryServer(value) {
            lastTimeout = null;
            suggest.show();
            $.post(url,
                { q: value },
                function (data) {
                    suggest.html(data);
                }
            );
        }

        box.dialog({ autoOpen: true, width: '800px', height: '600', modal: true });
    });

    var update_text = function (container) {
        var html = container.find("textarea").val().replace(/\n/g, "<br />");
        container.find(".onlyprint").html(html);
    };

    $("textarea").each(function () {
        var textarea_container = $("<div></div>").addClass("textarea_container");
        var screen_textarea = $(this).clone().addClass("noprint").bind('change', function () {
            update_text($(this).closest(".textarea_container"));
        });
        var print_textarea = $("<div></div>").addClass("onlyprint");

        textarea_container.append(screen_textarea).append(print_textarea);
        $(this).replaceWith(textarea_container);
        update_text(textarea_container);
    });

    $('.bookmarkpage').live("click", function () {
        var self = $(this);
        $.post("/Home/BookmarkPage",
            { Path: self.attr('rel'), Titulo: self.attr('title') },
            function (data) {  //Post to action
                if (data == 'BOOKMARKED') {
                    self.children("#b").attr("class", "icon-bookmark");
                }
                else if (data == 'UNBOOKMARKED') {
                    self.children("#b").attr("class", "icon-bookmark-2");
                }
                else {
                    ShowError(data);
                }
            }
        );
    });


    $('.showerror').each(function () {
        ShowError($(this).html());
    });

    $('.showsuccess').each(function () {
        ShowSuccess($(this).html());
    });


    $('#loadingDiv')
    .ajaxStart(function () {
        $(this).show();
    })
    .ajaxStop(function () {
        $(this).hide();
        doinitialize('.newelements'); // no funciona solo para la solapa de los tabs
        //setTimeout(function () { doinitialize('.newelements'); }, 500); //lo mando medio segundo despues porque hay veces que no llega a cargar todo
    });
    //$(document).ajaxStop(function () {
    //    //setTimeout(function () { doinitialize('.newelements'); }, 500); //lo mando medio segundo despues porque hay veces que no llega a cargar todo
    //    $( document ).ready(function() {
    //        doinitialize('.newelements');
    //    })
    //});
    //$(document).ajaxComplete(function () {
    //    doinitialize('.newelements');
    //});
    $.ajaxSetup({
        cache: false
    });

    $("#tabs").tabs({
        load: function () { doinitialize('.newelements'); }
    });

    // causa un doble request, esta mal
    //$.ajax({
    //    // url, data, etc.
    //    success: function () {
    //        doinitialize('.newelements');
    //    }
    //});
    $("form:not(.filegenerator)").on("submit", function () {
        if ($(document.activeElement).hasClass("superlistinput")) {
            $(document.activeElement).data("autocomplete")._trigger("change");
        }
        $.blockUI({ message: null });
        $('#loadingDiv').show();
    });

    AnimarNotificacion();

});

// several changes made to accept click and hover jsfiddle.net/xjuZ4/13/
var timeout = 500;
var closetimer = 0;
var ddmenuitem = 0;

function jsddm_open(myobj) {
    var submenu = myobj.find('ul');
    jsddm_canceltimer();
    jsddm_close();
    if (submenu) {
        ddmenuitem = submenu.css('visibility', 'visible');
        return false;
    }
    return true;
}
function jsddm_close() { if (ddmenuitem) ddmenuitem.css('visibility', 'hidden'); }
function jsddm_timer() { closetimer = window.setTimeout(jsddm_close, timeout); }
function jsddm_canceltimer() {
    if (closetimer) {
        window.clearTimeout(closetimer);
        closetimer = null;
    }
}
$(document).ready(function () {
    $('.jsddm > li').hoverIntent(function () { jsddm_open($(this)); }, jsddm_timer);
    $('.topmenulink').bind('click', function (e) {
        jsddm_open($(this).parent());
        e.stopPropagation();
    });
});

document.onclick = jsddm_close;

function parseLocalFloat(num) {
    if (!num) return 0;
    return parseFloat((num.replace(/\./g, "").replace(/ /g, "").replace("$", "").replace(",", ".")));
}

String.prototype.TryCInt = function (c) {
    return parseLocalFloat(this);
};


Number.prototype.formatMoney = function (c) {
    var d = ","; var t = ".";
    var n = this, c = isNaN(c = Math.abs(c)) ? 2 : c, d = d == undefined ? "," : d, t = t == undefined ? "." : t, s = n < 0 ? "-" : "", i = parseInt(n = Math.abs(+n || 0).toFixed(c)) + "", j = (j = i.length) > 3 ? j % 3 : 0;
    return s + (j ? i.substr(0, j) + t : "") + i.substr(j).replace(/(\d{3})(?=\d)/g, "$1" + t) + (c ? d + Math.abs(n - i).toFixed(c).slice(2) : "");
};

Number.prototype.formatNumber = function (c) {
    var d = ","; var t = ".";
    var n = this, c = isNaN(c = Math.abs(c)) ? 2 : c, d = d == undefined ? "," : d, t = t == undefined ? "." : t, s = n < 0 ? "-" : "", i = parseInt(n = Math.abs(+n || 0).toFixed(c)) + "", j = (j = i.length) > 3 ? j % 3 : 0;
    return s + (j ? i.substr(0, j) + t : "") + i.substr(j).replace(/(\d{3})(?=\d)/g, "$1" + t) + (c ? d + Math.abs(n - i).toFixed(c).slice(2) : "");
};


/* jQuery autohelp Copyright Dylan Verheul <dylan@dyve.net>
* Licensed like jQuery, see http://docs.jquery.com/License
* http://dyve.net/jquery/?autohelp
*/

$.fn.autohelp = function (t, o) {
    t = $(t); o = o || {};
    if (this.title != null || this.title != '') {
        this.focus(function () { t.html(this.title); t.show(); })
	        .blur(function () { t.hide(); });
    };
    return this;
};

function formatHora(hora) {
    if (hora == "") {
        return "";
    }

    if (hora.indexOf(":") < 0) {
        if (hora.length === 1) {
            return "0" + hora + ":00";
        }
        else if (hora.length === 2) {
            return hora + ":00";
        }
        else if (hora.length === 3) {
            return "0" + hora.substring(0, 1) + ":" + hora.substring(1, 3);
        }
        else if (hora.length === 4) {
            return hora.substring(0, 2) + ":" + hora.substring(2, 4);
        }
    }
    return hora;
}

function formatFecha(fecha, esfuturedatetime) {
    var today = new Date();
    if (fecha == "") {
        return "";
    }
    if (fecha.indexOf("/") < 0) {
        if (fecha > today.getDate() && !esfuturedatetime) {
            return fecha + "/" + (today.addMonths(-1).getMonth() + 1) + "/" + today.addMonths(-1).getFullYear();
        }
        else {
            return fecha + "/" + (today.getMonth() + 1) + "/" + today.getFullYear();
        }
    }
    else if ((fecha.split("/").length - 1) == 1) {
        var splitted = fecha.split("/");
        var target = new Date(today.getFullYear(), splitted[1] - 1, splitted[0]);
        if (esfuturedatetime) {
            if (target < today && (today.getMonth() - today.getMonth()) > 3) {
                return splitted[0] + "/" + splitted[1] + "/" + (today.getFullYear() + 1);
            }
        }
        else if (target > today) {
            return splitted[0] + "/" + splitted[1] + "/" + (today.getFullYear() - 1);
        }
        return splitted[0] + "/" + splitted[1] + "/" + today.getFullYear();
    }
    return fecha;
}

function formatMonth(fecha) {
    var today = new Date();
    if (fecha == "") {
        return "";
    }
    if (fecha.indexOf("/") < 0) {
        if (fecha > today.getMonth() + 1) {
            return fecha + "/" + (today.getFullYear() - 1);
        }
        else {
            return fecha + "/" + today.getFullYear();
        }
    }
    else if ((fecha.split("/").length - 1) == 1) {
        var splitted = fecha.split("/");
        if (splitted[1].length == 2) {
            return splitted[0] + "/20" + splitted[1];
        }
    }
    return fecha;
}

function formatComprobante(texto) {
    if (texto === "") {
        return "";
    }
    if (texto.indexOf("-") < 0) {
        if (texto.length == 1) {
            return texto.substring(0, 1).toUpperCase() + "-0000-00000000";
        }
        else if (texto.length >= 5) {
            return texto.substring(0, 1).toUpperCase() + "-" + texto.substring(1, 5) + "-" + pad(texto.substring(5, 13), 8, "0");
        }
    }
    else {
        var splitted = texto.split("-");
        if (splitted.length == 3) {
            return splitted[0].substring(0, 1).toUpperCase() + "-" + pad(splitted[1].substring(0, 4), 4, "0") + "-" + pad(splitted[2].substring(0, 8), 8, "0");
        }
    }
    return texto;
}

function pad(n, width, z) {
    z = z || '0';
    n = n + '';
    return n.length >= width ? n : new Array(width - n.length + 1).join(z) + n;
}

function dolistinput(el, OnSuccess, OnFailture) {
    el = $(el);
    //mejora la performace, tuve un problema pero resultaba ser porque no ser estaba destruyendo en div del miniedit
    var elprev = el.prev();
    var tieneExtraInfo = el.attr("data-extrainfo") == "true";
    var elExtraInfo = tieneExtraInfo ? el.parent().next(".masinfo") : null;
    el.autocomplete({
        minLength: 0,
        source: function (request, response) {
            $.ajax({
                url: '/' + el.data("controller") + '/Lista/',
                dataType: 'json',
                type: 'POST',
                data: getdatafromparams($(el).data('params'), "term", request.term),
                success: function (data) {
                    response(data);
                }
            });
        },
        select: function (event, ui) {
            elprev.val(ui.item.ID).change();
            if (OnSuccess != null)
                OnSuccess(el, ui.item);
            el.val(ui.item.Nombre);
            if (tieneExtraInfo) {
                elExtraInfo.show().attr('rel', '/' + el.data("controller") + '/extrainfo/' + ui.item.ID);
                // vuelvo a cargar el qtip
                elExtraInfo.qtip({
                    content: {
                        // Set the text to an image HTML string with the correct src URL to the loading image you want to use
                        text: '<img src="/content/images/loading.gif" alt="Cargando..." />',
                        ajax: {
                            url: '/' + el.data("controller") + '/extrainfo/' + ui.item.ID // Use the rel attribute of each element for the url to load
                        },
                        title: {
                            text: 'Más información - ' + elExtraInfo.text(), // Give the tooltip a title using each elements text
                            button: true
                        }
                    },
                    position: {
                        at: 'bottom center', // Position the tooltip above the link
                        my: 'top center',
                        viewport: $(window) // Keep the tooltip on-screen at all times
                    },
                    show: {
                        event: 'click',
                        solo: true, // Only show one tooltip at a time
                        effect: function () {
                            $(this).slideDown();
                        }

                    },
                    hide: 'unfocus',
                    style: {
                        classes: 'qtip-light qtip-shadow qtip-rounded'
                    }
                });
            }
            return false;
        },
        change: function (event, ui) {
            el.removeClass('superlistinvalid');
            if (!ui.item && (el.initialItem.ID !== elprev.val() || el.initialItem.Nombre !== el.val())) {
                var valoractual = $.trim(el.val());
                if (valoractual === "") {
                    elprev.val("0").change(); //no need to go validate
                    if (OnFailture) OnFailture(el);
                    if (tieneExtraInfo) elExtraInfo.hide();
                }
                else {
                    $.ajax({
                        url: '/' + el.data("controller") + '/ListaValidar/',
                        dataType: 'json',
                        type: 'POST',
                        data: getdatafromparams($(el).data('params'), "id", valoractual),
                        success: function (json) {
                            var valid = false;
                            if (json.length > 0) {
                                elprev.val(json[0].ID).change();
                                el.val(json[0].Nombre);
                                if (OnSuccess!=null)
                                    OnSuccess(el,json[0]);
                                if (tieneExtraInfo) {
                                    elExtraInfo.show().attr('rel', '/' + el.data("controller") + '/extrainfo/' + json[0].ID);
                                    elExtraInfo.qtip({
                                        content: {
                                            // Set the text to an image HTML string with the correct src URL to the loading image you want to use
                                            text: '<img src="/content/images/loading.gif" alt="Cargando..." />',
                                            ajax: {
                                                url: $(this).attr('rel') // Use the rel attribute of each element for the url to load
                                            },
                                            title: {
                                                text: 'Más información - ' + $(this).text(), // Give the tooltip a title using each elements text
                                                button: true
                                            }
                                        },
                                        position: {
                                            at: 'bottom center', // Position the tooltip above the link
                                            my: 'top center',
                                            viewport: $(window) // Keep the tooltip on-screen at all times
                                        },
                                        show: {
                                            event: 'click',
                                            solo: true, // Only show one tooltip at a time
                                            effect: function () {
                                                $(this).slideDown();
                                            }

                                        },
                                        hide: 'unfocus',
                                        style: {
                                            classes: 'qtip-light qtip-shadow qtip-rounded'
                                        }
                                    });
                                                                        
                                }
                                valid = true;
                            };
                            if (!valid) {
                                // it didn't match anything
                                el.addClass('superlistinvalid');
                                elprev.val("0").change();
                                if (OnFailture) OnFailture(el);
                                if (tieneExtraInfo) elExtraInfo.hide();
                            };
                            return false;
                        }
                    });
                    if (tieneExtraInfo) elExtraInfo.show();
                }
            }
        }
    })
    .data("autocomplete")._renderItem = function (ul, item) {
        return $("<li></li>")
          .data("item.autocomplete", item)
          .append('<a>' + item.Nombre + (item.ExtraInfo === null ? '' : '<div><i>' + item.ExtraInfo + '</i></div>') + '</a>')
          .appendTo(ul);
    };

    el.blur(function (event) {
        if ($.trim(el.val()) == "") {
            elprev.val("0").change();
            el.change();
            if (tieneExtraInfo) elExtraInfo.hide();
        }
        else if ((elprev.val() == "0" || elprev.val() == "") && (el.val() != "")) {
            el.data("autocomplete")._trigger("change");
        }
    });


    // lo marco como invalido luego de un submit tambien
    // y pongo el valor inicial
    if ((elprev.val() == "0" || elprev.val() == "") && (el.val() != "")) {
        el.addClass('superlistinvalid');
        if (tieneExtraInfo) elExtraInfo.hide();
        el.initialItem = { ID: -999, Nombre: "--force validate--" };
    }
    else {
        el.initialItem = { ID: elprev.val(), Nombre: el.val() };
    }

    // hago que se morfe el enter por dos motivos: 
    // * A veces se equivocan y mandar a guardar
    // * No le da el tiempo a cargarse antes del submit
    el.keypress(function (event) {
        if (event.keyCode == 13) {
            event.preventDefault();
        }
    });


    // cambie esta funcion por el blur anterior porque traia problemas cuando no habia cambiado nada y el valor era correcto
    //el.keyup(function (event) {
    //    if (event.keyCode != '13' && event.keyCode != '9') {
    //        elprev.val(0).change();
    //    }
    //});
}

function dosuperlistinput(el,OnSuccess,OnFailture) {
    dolistinput(el, OnSuccess, OnFailture);
    addbuttonautocomplete(el);
}

function doitemlistinput(el) {
    dolistinput(el);
}
function doSimpleAutocomplete(el) {
    el = $(el);
    el.autocomplete({
        minLength: 0,
        source: function (request, response) {
            $.ajax({
                url: '/' + el.data("controller") + '/Lista/',
                dataType: 'json',
                type: 'POST',
                data: getdatafromparams(el.data("params"), "term", request.term),
                success: function (data) {
                    response(data);
                }
            });
        },
        select: function (event, ui) {
            el.val(ui.item.Nombre);
            return false;
        }
    })
    .data("autocomplete")._renderItem = function (ul, item) {
        return $("<li></li>")
          .data("item.autocomplete", item)
          .append('<a>' + item.Nombre + '</a>')
          .appendTo(ul);
    };

    addbuttonautocomplete(el);

}



function getdatafromparams(params, termName, termValue) {
    var retval = {};
    for (var prop in params) {
        if (params[prop].toString().substring(0, 1) == "#") {
            // trae problemas cuando el ID contiene un punto. Ver http://stackoverflow.com/questions/605630/how-to-select-html-nodes-by-id-with-jquery-when-the-id-contains-a-dot
            //retval[prop] = $(params[prop]).val();
            // este tampoco porque hay veces que el selector es mas complejo, por ejemplo en RemitoItem
            //retval[prop] = $("input[id='" + params[prop].substring(1) + "']").val();
            retval[prop] = $(params[prop].replace(".", "\\.")).val();
        }
        else {
            retval[prop] = params[prop];
        }
    }
    retval[termName] = termValue;
    return retval;
}

function addbuttonautocomplete(el) {
    el = $(el);
    $('<button type="button">&nbsp;</button>')
        .attr('tabIndex', -1)
        .attr('title', 'Mostrar todos los items')
        .insertAfter(el)
        .button({
            icons: {
                primary: 'ui-icon-triangle-1-s'
            },
            text: false
        })
        .removeClass('ui-corner-all')
        .addClass('ui-corner-right ui-button-icon autocompletebutton')
        .click(function (event) {
            event.preventDefault();

            // close if already visible
            if (el.autocomplete('widget').is(':visible')) {
                el.autocomplete('close');
                return;
            }

            // work around a bug (likely same cause as #5265)
            $(this).blur();

            // pass empty string as value to search for, displaying all results
            el.autocomplete('search', '');
            el.focus();
        });
};


function htmlEncode(value) {
    return $('<div/>').text(value).html();
}

function htmlDecode(value) {
    return $('<div/>').html(value).text();
}

//function attachGenerateExcelButton(el) {
//    el = $(el);
//    $("<div class='floatGenerateExcelButton'><i class='icon-arrow-down-2' /> Bajar esta tabla</div>")
//        .insertAfter(el)
//        .click(function (event) {
//            generateExcel(el)
//        });

//    el.parent().hover(
//        function () {
//            //var o = el.position();
//            //el.prev(".floatGenerateExcelButton").css({ 'position': 'absolute', 'left': o.left, 'top': o.top }).show();
//            el.next(".floatGenerateExcelButton").show();
//        },
//        function () {
//            el.next(".floatGenerateExcelButton").hide();
//        }
//    );
//}


function generateExcel(el) {
    var clon = el.clone();
    clon.find(".filtrablerow").remove().end();
    clon.find(".tableFloatingHeader").remove().end();
    clon.find(".ui-button").remove().end();
    clon.find("tfoot").remove().end();
    var html = clon.wrap('<div>').parent().html();

    //add more symbols if needed...
    while (html.indexOf('á') != -1) html = html.replace(/á/g, '&aacute;');
    while (html.indexOf('é') != -1) html = html.replace(/é/g, '&eacute;');
    while (html.indexOf('í') != -1) html = html.replace(/í/g, '&iacute;');
    while (html.indexOf('ó') != -1) html = html.replace(/ó/g, '&oacute;');
    while (html.indexOf('ú') != -1) html = html.replace(/ú/g, '&uacute;');
    while (html.indexOf('º') != -1) html = html.replace(/º/g, '&ordm;');
    html = html.replace(/<td>/g, "<td>&nbsp;");

    window.open('data:application/vnd.ms-excel,' + encodeURIComponent(html));
}

jQuery.fn.loadOuter = function (url, callback) {
    var toLoad = $(this);
    $.get(url, function (data) {
        toLoad.replaceWith(data);
        if (callback != null && callback != undefined)
            callback();
    });
}

//si se usa mucha quizas hay que pasarla a site.js
$.fn.reverse = function () {
    return this.pushStack(this.get().reverse(), arguments);
};

// create two new functions: prevALL and nextALL. they're very similar, hence this style.
// ejemplo: self.prevALL('input[id$="__HHasta"]').first().val();
$.each(['prev', 'next'], function (unusedIndex, name) {
    $.fn[name + 'ALL'] = function (matchExpr) {
        // get all the elements in the body, including the body.
        var $all = $('body').find('*').andSelf();

        // slice the $all object according to which way we're looking
        $all = (name == 'prev')
     ? $all.slice(0, $all.index(this)).reverse()
     : $all.slice($all.index(this) + 1)
        ;
        // filter the matches if specified
        if (matchExpr) $all = $all.filter(matchExpr);
        return $all;
    };
});

jQuery.fn.exists = function () { return this.length > 0; }

String.prototype.contains = function (it) { return this.indexOf(it) != -1; };
if (typeof String.prototype.startsWith != 'function') {
    String.prototype.startsWith = function (str) {
        return this.slice(0, str.length) == str;
    };
}
if (typeof String.prototype.endsWith != 'function') {
    String.prototype.endsWith = function (str) {
        return this.slice(-str.length) == str;
    };
}


var _round = Math.round;
Math.round = function (number, decimals /* optional, default 0 */) {
    if (arguments.length == 1)
        return _round(number);

    var multiplier = Math.pow(10, decimals);
    return _round(number * multiplier) / multiplier;
}

function ProductoDataMasInfo(data) {
    if (data != null && data.ProductoID != 0 && !data.toString().indexOf("ERROR") == 0) {
        var retval = "Ubicación: " + data.Ubicacion + " <a class='miniedit' data-callback='ProductoRefreshInfo(" + data.ProductoID + "," + data.LugarID + ")' href='/Productos/CambiarUbicacion/" + data.ProductoID + "?LugarID=" + data.LugarID + "'>Cambiar</a><br>" + "Stock Actual: " + data.Remito.formatNumber();
        return retval;
    }
    return "";
}

function ProductoRefreshInfo(ProductoID, LugarID) {
    $('.info_' + ProductoID + '_' + LugarID).each(function () {
        var self = $(this);
        var url = "/Productos/DatosCompletos/" + ProductoID + "?LugarID=" + LugarID;
        $.get(url, function (data) {
            self.html(ProductoDataMasInfo(data));
        });
    })
}

function ShowInfo(mensaje) {
    toastr.info(mensaje)
}
function ShowWarning(mensaje) {
    toastr.warning(mensaje)
}
function ShowSuccess(mensaje) {
    toastr.success(mensaje)
}
function ShowError(mensaje) {
    toastr.error(mensaje)
}


function AgregarLinea(table) {
    //var table = $(this).closest('table');
    //var table = $("#latabla");
    var row = table.find("tbody tr:last").clone();

    row.find("input").each(function (i, el) {
        el = $(el);
        var id = el.attr("id");
        var name = el.attr("name");

        if (name.endsWith(".Index")) {
            // son los que tienen esto
            //<input type="hidden" name="Items.Index" value="@i" /> 
            // para poder agregar items aleatorios
            var nextIndexIndex = parseInt(el.val()) + 1;
            el.val(nextIndexIndex)
        }
        else if (id && name) {
            var lastPoint = id.indexOf("__");
            // bueso el _ sabiendo que como mínimo esta dos posiciones antes del __
            var firstPoint = lastPoint - 2;
            while (id.substring(firstPoint, firstPoint + 1) != '_') {
                firstPoint--;
            }

            if (firstPoint > 0 && lastPoint > 0 && firstPoint < lastPoint) {
                var index = id.substring(firstPoint + 1, lastPoint);
                var nextIndex = parseInt(index) + 1;
                el.attr("id", id.replace("_" + index + "__", "_" + nextIndex + "__"));
                el.attr("name", name.replace("[" + index + "]", "[" + nextIndex + "]"));
                if (el.attr("data-params")) {
                    el.attr("data-params", el.attr("data-params").replace("_" + index + "__", "_" + nextIndex + "__"));
                }
                // tengo que chequear si no tiene la clase donttouchonclone y dejarle el val en blanco
                if (!el.hasClass("donttouchonclone")) {
                    el.val("");
                }
            }
        }
    });
    row.find("a").each(function (i, el) {
        el = $(el);
        var id = el.attr("id");
        if (id ) {
            var lastPoint = id.indexOf("__");
            // bueso el _ sabiendo que como mínimo esta dos posiciones antes del __
            var firstPoint = lastPoint - 2;
            while (id.substring(firstPoint, firstPoint + 1) != '_') {
                firstPoint--;
            }

            if (firstPoint > 0 && lastPoint > 0 && firstPoint < lastPoint) {
                var index = id.substring(firstPoint + 1, lastPoint);
                var nextIndex = parseInt(index) + 1;
                el.attr("id", id.replace("_" + index + "__", "_" + nextIndex + "__"));
            }
        }
    });
    table.find("tbody").append(row);

    // reinicializo jQuery en los nuevos items
    row = table.find("tbody tr:last");
    row.find(".autocompletebutton").remove().end();
    row.addClass("newelements");

    var functionToCallToInitialize = table.data("initializefunction");
    if (functionToCallToInitialize && functionToCallToInitialize != "") {
        window[functionToCallToInitialize](".newelements");
    }

    doinitialize(".newelements");


}


(function ($) {
    $.fn.extend({
        animateNumber: function (options) {

            var defaults = {
                decimals: 2,
                prefix: ""
            }

            var options = $.extend(defaults, options);

            return this.each(function () {
                var el = $(this);
                var toNumber = parseLocalFloat(el.html());
                var fromNumber = toNumber - 10000;
                var vel = 1; var loop;
                if (fromNumber < 0) fromNumber = 0;
                var velInicial = toNumber - fromNumber;
                (loop = function () {
                    fromNumber += Math.max(Math.round((toNumber - fromNumber) / 10), 1);
                    el.html(options.prefix + fromNumber.formatNumber(options.decimals));
                    vel = Math.max(velInicial - toNumber - fromNumber, 1);
                    if (fromNumber > toNumber) {
                        el.html(options.prefix + toNumber.formatNumber(options.decimals));
                        return;
                    }
                    setTimeout(loop, vel);
                })();
            })
        }
    });
})(jQuery);

function AnimarNotificacion() {
    var counter = $("#notification-counter");
    var cant = parseInt(counter.html());
    if (cant > 0) {
        counter
        .css({ opacity: 0 })
        .css({ top: '-10px' })
        .show()
        .animate({ top: '-2px', opacity: 1 })
    }
}


