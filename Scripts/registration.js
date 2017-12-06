
       $.validator.setDefaults({
            submitHandler: function (form) {
                if($(form).valid()) {
                    $(form).ajaxSubmit();
                }
            }
        });

        var validator = $("#registrationForm"),
        status = $(".status");
        
        $.validator.addMethod("accept", function (value, element, param) {
            return value.match(new RegExp("^" + param + "$"));
        }, 'please enter a valid set of characters');


        $(document).ready(function () {

            /*$('#mFaEnabled').change(function(){
                $(this).validate({
                    if ($('#mFaEnabled[value="Yes"]:checked')) {
                        alert("Yes");
                        $('#MOBILE_NUMBER').attr('required');
                        MOBILE_NUMBER: "required"
                    } else {
                        $('#MOBILE_NUMBER').removeAttr('required');
                        MOBILE_NUMBER: ""
                    }
                });
            });*/

            $("#registrationPreFlightSubmit").on("click",
                function () {
                    registrationPreFlightSubmit();
                }
            );

            $("#registerUser").on("click",
                function () {
                    registerUser();
                }
            );

            // set up the masking for the phone and mobile
            $("#phoneNumber").mask("(999) 999-9999");
            $("#mobileNumber").mask("(999) 999-9999");
            $("#phoneExtension").mask("9999999999");                    
                        
             $('#commentsAgents').bind('copy', function (e) { e.preventDefault(); return false; });
            $('#commentsAgents').bind('paste', function (e) { e.preventDefault(); return false; });
        }); //END DOC READY

        function registrationPreFlightSubmit() {
            console.log("Registration preflight fired.");

            // clear the preflight error box
            //if ($("#errorSummary:visible")) {
            //    $("#errorMessage").html('');
            //    $("#errorSummary").toggle();
            //}

            // Validate the email address being entered.
            validateEasyPayUser(true, $("#preflightUsername").val());
        };
        function registerUser() {
            console.log("registerUser fired");
            validator.validate();

            var user = {
                "UserSysId": "0",
                "EmailAddress": $("#userName").val(),
                "Password": $("#password").val(),
                "FirstName": $("#firstName").val(),
                "LastName": $("#lastName").val(),
                "CompanyId": $("#companyId").val(),
                "PhoneNumber": $("#phoneNumber").val().replace(/\D/g, ''),
                "PhoneExtension": $("#phoneExtension").val().replace(/\D/g, ''),
                "MobileNumber": $("#mobileNumber").val().replace(/\D/g, ''),
                "Timezone": $("#timezone").val(),
                "IsLocked": "0",
                "IsActive": "0",
                "Notes": $("#commentsAgents").val(),
                "UserGuid": "",
                "MfaEnabled[]": $("#mFaEnabled:checked").val(),
                "MfaPhoneNumber": $("#mobileNumber").val()
            };

            console.log("User to register: ", user);


            var user = {
                "UserSysId": "0",
                "EmailAddress": $("#userName").val(),
                "Password": $("#password").val(),
                "FirstName": $("#firstName").val(),
                "LastName": $("#lastName").val(),
                "CompanyId": $("#companyId").val(),
                "PhoneNumber": $("#phoneNumber").val(),
                "PhoneExtension": $("#phoneExtension").val(),
                "MobileNumber": $("#mobileNumber").val(),
                "Timezone": $("#timezone").val(),
                "IsLocked": "0",
                "IsActive": "0",
                "Notes": $("#commentsAgents").val(),
                "UserGuid": "",
                "MfaEnabled[]": $("#mFaEnabled:checked").val(),
                "MfaPhoneNumber": $("#mobileNumber").val()
            };

            var endpointUrl = "";
            if (localStorage.getItem("CreateAuth0Record")) {
                localStorage.removeItem("CreateAuth0Record")
                endpointUrl = "/api/v1/Administration/AddUser?addAuth0=true";
            } else {
                endpointUrl = "/api/v1/Administration/AddUser?addAuth0=false";
            }

            $.ajax({
                type: "POST",
                accepts: "application/json",
                contentType: 'application/json',
                url: endpointUrl,
                data: JSON.stringify(user),
                success: function (user) {
                    var serverResponse = JSON.parse(user.results);
                    window.location.href = "/home/index?emailAddress=" + serverResponse.USER_NAME;
                },
                error: function (errorMsg) {
                    console.log("Something broke: ", errorMsg);
                },
                complete: function () {
                }
            });
        };

        function validateEasyPayUser(sso, email) {
            var apiUrl = "/api/v1/Authorization/ValidateEmail?SSO=true&email=" + email.toLowerCase();

            $.ajax({
                url: apiUrl,
                type: "POST",
                success: function (data) {
                    if (data) {
                        console.log("Server response: ", data);
                        var serverResponse = JSON.parse(data.results);

                        if (typeof serverResponse.isInternal !== "undefined") {
                            if (serverResponse.isInternal) {
                                $("#errorMessage").html("Please contact the helpdesk at XXX-XXX-XXXX for assistance with access using an Internal email address.");
                                $("#errorSummary").show();
                            }
                        } else if (typeof serverResponse.Auth0Blocked !== "undefined") {
                            var errorMsg = "no particular reason";
                            if (serverResponse.Reason !== '') {
                                errorMsg = serverResponse.Reason;
                            }
                            $("#errorMessage").html("This account has been blocked due to " + errorMsg + ". Please contact the helpdesk at XXX-XXX-XXXX for assistance.");
                            $("#errorSummary").show();
                        } else if (typeof serverResponse.auth0Id !== "undefined") {
                            $("#registrationPreFlight").hide();
                            $("#registrationFormControl").show();
                            $("#userName").val(serverResponse.emailAddress).attr("disabled", "disabled");
                            $("#hid_userName").val(serverResponse.emailAddress);
                            $("#firstName").val(serverResponse.firstName).attr("disabled", "disabled");
                            $("#hid_firstName").val(serverResponse.firstName);
                            $("#lastName").val(serverResponse.lastName).attr("disabled", "disabled");
                            $("#hid_lastName").val(serverResponse.lastName);
                            $("#companyId").val(serverResponse.companyName).attr("disabled", "disabled");
                            $("#hid_companyId").val(serverResponse.companyName);
                            var sanitizedPhone = serverResponse.phoneNumber.replace(/\D/g, '');
                            $("#phoneNumber").val(sanitizedPhone.substring(1, sanitizedPhone.length)).mask("(999) 999-9999").attr("disabled", "disabled");
                            $("#hid_phoneNumber").val(sanitizedPhone.substring(1, sanitizedPhone.length));
                        } else {
                            // They exist in both Auth0 and EasyPay...but they're not logged in.
                            window.location.href = "/home/index?emailAddress=" + email;
                        }
                    } else {
                        // Show everything...records will need to be added to both systems.
                        console.log("No existing user record found in either Auth0 or EasyPay");
                        $("#registrationPreFlight").hide();
                        $("#passwordRow").show();
                        $("#registrationFormControl").show();
                        $("#userName").val($("#preflightUsername").val());
                        $("#userName").attr("disabled", "disabled");
                        $("#hid_userName").val($("#preflightUsername").val());
                        localStorage.setItem("CreateAuth0Record", "true");
                    }
                },
                error: function (textStatus, errorThrown) {
                    console.log("Something happened: ", errorThrown);
                    return null;
                },
                complete: function () { }
            });
        }


        validator.validate({
                rules: {
                    userName: {
                        required: true,
                        email: true,
                        minlength: 1,
                        maxlength: 50,
                        accept: '/^[a-z0-9._%+-]+\u0040[a-z0-9.-]+\.[a-z]{2,3}$'
                    },
                    firstName: {
                        required: true,
                        minlength: 1,
                        maxlength: 50,
                        accept: '[a-zA-Z]*'
                        }
                    },
                    lastName: {
                        required: true,
                        minlength: 1,
                        maxlength: 50,
                        accept: '[a-zA-Z]*'
                    },
                    password: {
                        required: true,
                        minlength: 1,
                        maxlength: 50,
                        accept: '/^((?=.*?[A-Z])(?=.*?[a-z])(?=.*?\d)|(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[^a-zA-Z0-9])|(?=.*?[A-Z])(?=.*?\d)(?=.*?[^a-zA-Z0-9])|(?=.*?[a-z])(?=.*?\d)(?=.*?[^a-zA-Z0-9])).{8,}$/'
                    },
                    confirmPassword: {
                        required: true,
                        minlength: 1,
                        maxlength: 50,
                        equalTo: "#password",
                        accept: '/^((?=.*?[A-Z])(?=.*?[a-z])(?=.*?\d)|(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[^a-zA-Z0-9])|(?=.*?[A-Z])(?=.*?\d)(?=.*?[^a-zA-Z0-9])|(?=.*?[a-z])(?=.*?\d)(?=.*?[^a-zA-Z0-9])).{8,}$/'
                    },
                    timezone: {
                        required: true,
                        minlength: 1,
                        maxlength: 10
                    },
                    companyId: {
                        required: true,
                        minlength: 1,
                        maxlength: 50
                    },
                    phoneNumber: {
                        required: true,
                        minlength: 9,
                        maxlength: 16
                    },
                    phoneExtension: {
                        required: false,
                        maxlength: 10,
                        accept: '[0-9]*'
                    },
                    mobileNumber: {
                        minlength: 9,
                        maxlength: 16,
                        required: {
                            depends: function (element) {
                                if ($('#mFaEnabled:checked').val() === 'true') {
                                    return true;
                                } else {
                                    return false;
                                }
                            }
                        },
                        accept: '[0-9]*'
                        /*
                        required: function(element) {
                            //return $('#mFaEnabled[value="Yes"]:checked')
                            if($('#mFaEnabled:checked').val() === 'Yes'){
                                $('#MOBILE_NUMBER').attr('required');
                                return true;
                            } else {
                                $('#MOBILE_NUMBER').removeAttr('required');
                                return false;
                            }
                        }*/
                    }
                },
                messages: {
                    firstName: {
                        required: "Please enter your first name",
                        minlength: "Your first name can not be shorter than 1 character",
                        maxlength: "Your first name can not be longer than 50 characters"
                    },
                    lastName: {
                        required: "Please enter your last name",
                        minlength: "Your last name can not be shorter than 1 character",
                        maxlength: "Your last name can not be longer than 50 characters"
                    },
                    timezone: "Please select your timezone",
                    companyId: {
                        required: "Please enter your Company Name",
                        maxlength: "Your company name can not be longer than 50 characters"
                    },
                    phoneNumber: {
                        required: "Please enter a valid phone number"
                    },
                    mobileNumber: {
                        required: "For Multi-Factor Authentication a mobile number must be included"
                    },
                    password: {
                        required: "Please provide a password",
                        minlength: "Your password must be at least 5 characters long"
                    },
                    confirmPassword: {
                        required: "Please provide a password",
                        minlength: "Your password must be at least 5 characters long",
                        equalTo: "Please enter the same password as above"
                    },
                    phoneExtension: {
                        maxlength: "Your extension can not be longer than 10 numbers",
                    },
                    userName: {
                        required: "Please enter a valid email address",
                        maxlength: "Your Email Address can not be longer than 50 characters"
                    }
                },
                errorElement: "em",
                errorPlacement: function (error, element) {
                    // Add the help-block class to the error element
                    error.addClass("help-block text-danger font-weight-bold");

                    // Add `has-feedback` class to the parent div.form-group
                    // in order to add icons to inputs
                    element.parents(".errorCheck").addClass("has-feedback");

                    if (element.prop("type") === "radio") {
                        error.insertAfter(element.parent("label"));
                    } else {
                        error.insertAfter(element);
                    }

                    // Add the span element, if doesn't exists, and apply the icon classes to it.
                    if (!element.next("span")[0]) {
                        $("<span class='fa fa-times form-control-feedback'></span>").insertAfter(element);
                    }
                },
                success: function (label, element) {
                    // Add the span element, if doesn't exists, and apply the icon classes to it.
                    if (!$(element).next("span")[0]) {
                        $("<span class='fa fa-check form-control-feedback'></span>").insertAfter($(element));
                    }
                },
                highlight: function (element, errorClass, validClass) {
                    $(element).parents(".errorCheck").addClass("has-danger").removeClass("has-success");
                    $(element).next("span").addClass("fa-times").removeClass("fa-check");
                },
                unhighlight: function (element, errorClass, validClass) {
                    $(element).parents(".errorCheck").addClass("has-success").removeClass("has-danger");
                    $(element).next("span").addClass("fa-check").removeClass("fa-times");
                }/*,

                submitHandler: function(form) {
                    $.ajax({
                        success: function(response) {
                            alert("submitted!");
                        }            
                    });  
                }*/
            });
            $("#resetForm").click(function(){
                $("input").each(function(){
                    if($(this).is('[readonly]') == false && $(this).prop('disabled',false)){
                        $(this).val(null);
                    }
                });
            });