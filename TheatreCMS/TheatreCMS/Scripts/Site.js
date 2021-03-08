// Script for shrinking logo
// Add two variables for the logo and menu and wrapped the shrinkFunction in an if statement that checks to make sure they are present before calling them.

var navBarLogo = document.getElementById("logo");
var navBarMenu = document.getElementById("menu");

if (navBarLogo != null || navBarMenu != null) { 
    window.onscroll = function () { shrinkFunction() };
}

    function shrinkFunction() {
        if (document.body.scrollTop > 50 || document.documentElement.scrollTop > 50) {
            document.getElementById("logo").style.height = "50px";
            document.getElementById("menu").style.padding = " 1px 20px";
        } else {
            document.getElementById("logo").style.height = "90px";
            document.getElementById("menu").style.padding = "20px";
        }
    }


//Simple welcome message that prints to the console on App Start
console.log("Welcome to the theatre!");

//Script for Landing Page

if (document.getElementById("main-carousel") != null) {
    var slideIndex = 1;

    showSlides(slideIndex);

    function changeSlide(n) {
        showSlides(slideIndex += n);
    }

    function currentSlide(n) {
        showSlides(slideIndex = n);
    }

    function showSlides(n) {
        var i;
        var slides = document.getElementsByClassName("slides");
        var dots = document.getElementsByClassName("dot");
        if (n > slides.length) { slideIndex = 1 }
        if (n < 1) { slideIndex = slides.length }
        for (i = 0; i < slides.length; i++) {
            slides[i].style.display = "none";
        }
        for (i = 0; i < dots.length; i++) {
            dots[i].className = dots[i].className.replace(" active", "");
        }
        slides[slideIndex - 1].style.display = "block";
        dots[slideIndex - 1].className += " active";
    }
};

//Script for ~/Photo/Index modal

//============================================================================  Script for ~/Photo/Index page  =====================================================

// This script handles displaying and animating the photo modal 

$('#photo-modal').on('click', function () {
    console.log("modal clicked");
    $('#photo-modal').toggleClass('photo-modal--animation');
    $('body').css('overflow', 'auto')
});
function ShowModel(id) {
    $('#photo-modal').toggleClass('photo-modal--animation');
    $("body").css('overflow', 'hidden') // removes scrollbar when modal is open
    document.getElementById("photo-modal").style.display = "flex";
    document.getElementById("photo-modal--content").src = document.getElementById("photo-index-img-" + id).src;
    document.getElementById("photo-modal").onclick = function () {
        $('.photo-modal').fadeToggle(600);
    }
}
//End script for ~/Photo/Index modal

// =============================================================================== End Script for Photo/Index page =============================================================


// **************************************************************************** Begin Script for Photo Dynamic Loading *********************************************************************
// ===================================================================================Photo/Index page =====================================================================================

// This script handles the dynamic scrolling feature of the Photo/Index page.
// A set of photos is retrieved from the database, then when the vertical scrollbar reaches the bottom,
// a new set of photos is retrieved.

if (document.getElementById("scroll-container") != null) {
    // ajaxCompleted is used to ensure that the getPhotos function is only called once per ping to the server.
    var ajaxCompleted = true;
    $(document).ready(PhotoScroll());

    function PhotoScroll() {
        var pageIndex = 0;
        // The pageSize variable can be changed to alter the number of retrieved items
        var pageSize = 20;

        // this block calls the getData function when the vertical scrollbar reaches the bottom,
        // and loads the next set of photos.
        $(document).ready(function () {
            getPhotos(pageIndex, pageSize);
            pageIndex++;
            $(window).scroll(function () {
                // window scrolltop is rounded up with math.ceil() because it was returning inconsistent values. That's also why it's set to >= instead of ==
                if (Math.ceil($(window).scrollTop()) >=
                    $(document).height() - $(window).height() && ajaxCompleted) {
                    getPhotos(pageIndex, pageSize);
                    pageIndex++;
                }
            });
        });
    };

    // This function makes an AJAX call to the GetPhotos action method, sending pageIndex and pageSize as arguments.
    // The next set of photos is retrieved from the database and returned. When addPhotoRows is called, the photos are added to the Index page.
    function getPhotos(pageIndex, pageSize) {
        console.log("index: " + pageIndex + " pagesize: " + pageSize + " photos.length: "/* + photos.length*/);
        ajaxCompleted = false;
        $.ajax({
            type: 'GET',
            url: '/Photo/GetPhotos',
            data: { "pageIndex": pageIndex, "pageSize": pageSize },
            dataType: 'json',
            success: function (photos) {
                console.log("%index: " + pageIndex);
                addPhotoRows(photos);
            },
            beforeSend: function () {
                $("#progress").show();
            },
            complete: function () {
                $("#progress").hide();
            },
            error: function () {
                alert("Error while retrieving data!");
            }
        });
    };

    function addPhotoRows(photos) {
        if (photos != "[]") {
            photos = jQuery.parseJSON(photos);
            for (var i = 0; i < photos.length; i++) {
                $("table").append("<tr class='tr-styling scroll--container'>" +
                    // This td is for the photo
                    "<td class='td-styling'> <img id='photo-index-img-" +
                    photos[i].PhotoId + "' onclick='ShowModel(" +
                    photos[i].PhotoId + ")' class='thumbnail_size photo-index-img' src='/photo/displayphoto/" +
                    photos[i].PhotoId + "' }) /></td>" +
                    "<td class='td-styling'>" + photos[i].OriginalHeight + "</td>" +
                    "<td class='td-styling'>" + photos[i].OriginalWidth + "</td>" +
                    "<td class='td-styling'>" + photos[i].Title + "</td>" +
                    "<td class='td-styling'>" +
                    "<a href = '/photo/Edit/" + photos[i].PhotoId + "'>Edit | </a>" +
                    "<a href = '/photo/Details/" + photos[i].PhotoId + "'>Details | </a>" +
                    "<a href = '/photo/Delete/" + photos[i].PhotoId + "'>Delete</a>" +
                    "</td>" +
                    "</tr>")
            };
            ajaxCompleted = true;
        };
    };
};
// End infinite scrolling for Photo/Index page

////Script for sticky navbar
//window.onscroll = function () { stickyNav() };
//var menu = document.getElementById("menu");
//var sticky = menu.offsetTop;

//function stickyNav() {
//    if (window.pageYOffset >= sticky) {
//        menu.classList.add("sticky")
//    } else {
//        menu.classList.remove("sticky");
//    }
//}

// **************************************************************************** End Script for Photo Dynamic Loading *********************************************************************
// ========================================================================================= Photo/Index page ==============================================================================


// **************************************************************************** Begin script for Bulk Add **********************************************************************
// ============================================================================= CalendarEvents/BulkAdd ========================================================================
//      This script applies to the CalendarEvents/BulkAdd page. Its purpose is to allow an admin to create and edit multiple calendar events based
//      off of a start date, end date, show start time, day(s) of the week that shows will occur, and interval of weeks between shows.
//      When the user is satisfied with their list, they can then submit it to the database.
//      It uses moment.js to handle dates and times, and uses jQuery's AJAX method to pass data to and from the controller.


// This class represents a single calendar event.
// CalendarEvent properties that are capitalized match the properties in the MVC CalendarEvent model. 
// They must be verbatim in order for the JSON deserializer to work correctly
{ // Inserted braces to resolve uncaught syntaxerror
    
    class CalendarEvent {
        constructor(startDate, endDate, dayOfWeek, startTime) {
            this.Title = $("#generate__production-field").children("option").filter(":selected").text();
            this.ProductionId = $("#generate__production-field").val();
            this.StartDate = startDate;
            this.EndDate = endDate;     // events are never longer than one day, but to match the database, EndDate is the same date as StartDate with it's time advanced by the runtime of the production.
            this.dayOfWeek = dayOfWeek;
            this.startTime = startTime;
        }
    } 
    if ($("#generate-showtimes-section") != null) {
        var reviewEvents = [];                                // reviewEvents is used to store the complete list of calendar event objects, as they are added from generatedEvents. It's then passed to the back end.
        var runtime = 0                                     // runtime stores the length of a given event in minutes. It's then used to incrememnt the event's time and create a second event that marks the end time of the production.

        //      When a production is selected from the dropdown, an ajax call is made sending that production's start date, end date, productionId, and runtime.
        //      The start date and end date dropdowns are then autofilled, and the runtime variable is set.
        $("#generate__production-field").change(function () {
            var productionId = $("#generate__production-field").val();
            if ($(this).val() == '') {
                $('#bulkaddform')[0].reset();
                $('#evening-time').html("");
                $('#matinee-time').html("");
            }
            $.ajax({
                method: 'GET',
                url: '/CalendarEvents/GetProduction',
                data: { "productionId": productionId },
                dataType: 'json',
                success: function (data) {
                    if (data != "[]") {
                        let production = jQuery.parseJSON(data);
                        let openingDay = production[0].OpeningDay.substr(0, 10); // This removes the time of day and leaves only the date
                        let closingDay = production[0].ClosingDay.substr(0, 10);
                        $("#generate__start-date-field").val(openingDay);
                        $("#generate__end-date-field").val(closingDay);
                        runtime = production[0].Runtime;
                    if (production[0].ShowtimeMat == null) {
                        $('#matinee').attr('disabled', true);
                        $('#matinee-time').html("");
                        }
                    else {
                        $('#matinee').attr('disabled', false);
                        $("#matinee-time").html(moment(production[0].ShowtimeMat).format('h:mm a'));
                         }
                    if (production[0].ShowtimeEve == null) {
                        $('#evening').attr('disabled', true);
                        $('#evening-time').html("");
                        }
                    else {
                        $('#evening').attr('disabled', false);
                        $("#evening-time").html(moment(production[0].ShowtimeEve).format('h:mm a'));
                        }
                    }
                },
                error: function () {
                    alert("Error while retrieving data!");
                }
            });
        });

        //    This function does some traffic control. Once the generate button is clicked, the generatedEvents list is populated, and a modal pops up showing the list of dates that were added, 
        //    and offers yes and no buttons which allow the user to either go back and change their parameters
        //    or to append these showtimes to a final list to be reviewed and edited before being submitted 
        $("#generate-button").click(function () {
            $('.bulk-add_review-row').unbind('mouseenter mouseleave');

            var modal = $('#bulk-add-modal'),
                yesBtn = $('#bulk-add-modal_yes'),
                noBtn = $('#bulk-add-modal_no'),
                reviewShowtimes = false,                    // This variable is used to determine whether to render the list of times in the modal, or to append it to the review section.
                generatedEvents = generateShowtimes();
            if (generatedEvents.length < 1) {                     // If the list is empty, the user didn't select all the required parameters.
                return;
            }
            modal.show();
            createTable(generatedEvents, reviewEvents, reviewShowtimes);

            noBtn.off('click');                             // The .off() and .one() methods are to prevent event handlers from stacking up.
            noBtn.one("click", function () {
                modal.hide();
                $('.bulk-add_modal-row').remove();          // This clears all the entries from the modal table so they won't stack every time the Generate button is clicked.
            });
            yesBtn.off("click");
            yesBtn.one("click", function () {               // When the yes button is clicked, the modal disappears and clears its entries. The showtimes are appended to the reviewEvents list, and displayed in the review showtimes section. 
                modal.hide();
                reviewShowtimes = true;
                $('.bulk-add_modal-row').remove();
                $('.bulk-add_review-row').remove();         // The Review Showtimes list is generated from the reviewEvents list each time yes is clicked, so it needs to be cleared
                reviewEvents.push.apply(reviewEvents, generatedEvents);
                reviewEvents = reviewEvents.sort((a, b) => a.StartDate - b.StartDate);
                createTable(generatedEvents, reviewEvents, reviewShowtimes);
            });
        });

        //      This function takes the user's input and performs calculations to generate a list of events sorted by ascending date. It returns to the generatedEvents list.
        function generateShowtimes() {
            let startDate = moment($("#generate__start-date-field").val()),
                endDate = moment($("#generate__end-date-field").val()),
                eventDate = startDate,
                dateRange = endDate.diff(startDate, 'days'),
                interval = $("#interval").children("option").filter(":selected").val(),
                generatedEvents = [];
            let startTimes = [];                                          // This array holds each selected start time. An event is created for each start time on any given day.
            if ($('#matinee').is(':checked')) {
                startTimes.push($('#matinee-time').text());
            }
            if ($('#evening').is(':checked')) {
                startTimes.push($('#evening-time').text());
            }
            if ($('#custom-time').val() != "") {
                startTimes.push($('#custom-time').val());
            }
            if (startTimes.length == 0) {
                alert("Please select a start time");
            }

            let days = [];                                   // This array takes each selected day of the week. For each day, within each eligible week, events will be created. 
            if ($('#sunday').is(':checked')) {
                days.push(0);
            }
            if ($('#monday').is(':checked')) {
                days.push(1);
            }
            if ($('#tuesday').is(':checked')) {
                days.push(2);
            }
            if ($('#wednesday').is(':checked')) {
                days.push(3);
            }
            if ($('#thursday').is(':checked')) {
                days.push(4);
            }
            if ($('#friday').is(':checked')) {
                days.push(5);
            }
            if ($('#saturday').is(':checked')) {
                days.push(6);
            }
            if (days.length == 0) {
                alert("Please select at least one day")
                return (generatedEvents);
            }



            // This block calculates all eligible days, and creates an event for each showtime selected.
            // For each day selected, for each eligible week between the start date and end date that the day occurs, for each start time selected, an event is created.
            for (i = 0; i < days.length; i++) {
                if (days[i] < startDate.day()) {
                    days[i] += 7;
                }
                startDate.day(days[i]);
                eventDate = startDate; //refreshes the event date
                for (j = days[i]; j <= dateRange + 7; j += 7 * interval) {
                    if (eventDate.isBetween(startDate, endDate, undefined, '[]')) { //check for the eventDate to be within start and end date. The '[]' argument sets it to be inclusive of the start and end date.
                        for (k = 0; k < startTimes.length; k++) {
                            let hr = parseInt(startTimes[k].substr(0, startTimes[k].indexOf(':'))),       // startTimes are all strings, and Moment.js needs ints to add a time of day to a moment.
                                min = parseInt(startTimes[k].substr(startTimes[k].indexOf(':') + 1, 2)),  // This parses the the string "11:30 am" for example and creates a variable for the hr, the minute, and am or pm.
                                amOrPm = startTimes[k].slice(-2).toUpperCase();
                            if (amOrPm == 'PM' && hr < 12) {
                                hr += 12;
                            }
                            eventDate.hour(hr).minute(min);
                            var endTime = moment(eventDate);
                            endTime.add(runtime, "minutes")
                            const event = new CalendarEvent(moment(eventDate), endTime, eventDate.format('dddd'), startTimes[k]);
                            generatedEvents.push(event);
                        }
                    }
                    eventDate.add((7 * interval).toString(), 'days').format('ll'); //increments the event date to the next eligible date
                }
                startDate = moment($("#generate__start-date-field").val());
                eventDate = startDate;
            }
            return generatedEvents.sort((a, b) => a.StartDate - b.StartDate);
        }


        //this function generates a table displaying the list of events created in the generateShowTimes() function.
        //depending on the state of the reviewShowtimes variable, it will create the table in either the modal or the 'review showtimes' section.
        function createTable(generatedEvents, reviewEvents, reviewShowtimes) {
            // this block creates a table in the modal
            if (reviewShowtimes != true) {
                var table = document.getElementById("modal-table"),
                    row = table.insertRow();
                row.className = 'bulk-add_modal-row';
                for (i = 0; i < generatedEvents.length; i++) {
                    var cell = row.insertCell();
                    cell.innerHTML = generatedEvents[i].StartDate.format('ll');
                    cell = row.insertCell();
                    cell.innerHTML = generatedEvents[i].dayOfWeek;
                    cell = row.insertCell();
                    cell.innerHTML = generatedEvents[i].startTime;
                    row = table.insertRow();
                    row.className = 'bulk-add_modal-row';
                }
                document.getElementById('bulk-add-modal_content').appendChild(table);
            }

            // this block creates a table in the review showtimes section
            if (reviewShowtimes == true) {
                $("#review-showtimes-section").show();
                var table = document.getElementById("showtimes-table"),
                    row = table.insertRow();
                row.className = 'bulk-add_review-row';
                for (i = 0; i < reviewEvents.length; i++) {
                    var cell = row.insertCell();
                    if (typeof reviewEvents[i].StartDate != "string" && reviewEvents.length > 0) {
                        cell.innerHTML = reviewEvents[i].StartDate.format('ll');
                    }
                    cell = row.insertCell();
                    cell.innerHTML = reviewEvents[i].dayOfWeek;
                    cell = row.insertCell();
                    cell.innerHTML = reviewEvents[i].startTime;
                    row = table.insertRow();
                    row.className = 'bulk-add_review-row';
                }
                document.getElementById('showtimes-container').appendChild(table);    // generates the table in html.
                deleteRowFeature();
            }
        }

        // this function creates a delete button when a row is hovered over in the review showtimes section.
        // When it's clicked, it removes the corresponding row, and deletes the event from the master list
        function deleteRowFeature() {
            let row = $('.bulk-add_review-row');
            row.off('hover');                           // clears hover event handlers. prevents events from stacking
            row.hover(function () {                     // when a row is hovered over, a delete button is created, and the index of that row is recorded and used to remove that entry from the master list
                let button = $('<button type="submit" class="bulk-add_delete">Delete</button>')
                    .hide().fadeIn(1200);
                let rowIndex = $('tr').index(this) - 2; // targets the specific row to be deleted 
                $(this).append(button);
                button.click(function () {             // when the delete button is clicked, the row is removed from the table, and the corresponding event is removed from the master list.
                    button.closest('tr').remove();
                    reviewEvents.splice(rowIndex, 1);
                })
            }, function () {                          // this removes the delete button when the mouse stops hovering over that row.
                $('.bulk-add_delete').remove();
            });
        }
        $('#bulk-add_submit').off('click');
        $('#bulk-add_submit').click(submitEvents);

        function submitEvents() {
            for (var i = 0; i < reviewEvents.length; i++) {
                if (typeof reviewEvents[i].StartDate == "object") {    //this checks that .format is only applied to items that haven't yet been formatted.
                    reviewEvents[i].StartDate = reviewEvents[i].StartDate.format('lll');
                    reviewEvents[i].EndDate = reviewEvents[i].EndDate.format('lll');
                }
            }
            var data = JSON.stringify(reviewEvents);
            $.ajax({
                method: 'POST',
                url: '/CalendarEvents/BulkAdd',
                data: { 'jsonString': data },
                success: function () {
                    if (reviewEvents.length == 0) {
                        alert("Events already added")
                    }
                    else {
                        alert('Events Added!');
                        reviewEvents = [];
                    }
                },
                error: function () {
                    alert("Error while posting data!");
                }
            });
        };
    }
}
// *************************************************************************** End Script for Bulk Add ******************************************************************************************
// ===========================================================================  CalendarEvents/BulkAdd  =====================================================================================================


// ************************************************************************ Favorite Cast Member Button ************************************************************************

function FavoriteCastMember(castMemberId) {
    var paragraphId = "fb-" + castMemberId;
    var likeBtn = document.getElementById(paragraphId);

    $.ajax({
        method: 'POST',
        url: '/Account/ToggleFavoriteCastMembers/',
        data: { 'id': castMemberId },

    })

    if (likeBtn.classList.contains("fb-not-favorited")) {
        likeBtn.classList.remove("fb-not-favorited");
        likeBtn.classList.add("fb-favorited");
    } else if (likeBtn.classList.contains("fb-favorited")) {
        likeBtn.classList.remove("fb-favorited");
        likeBtn.classList.add("fb-not-favorited");
    }
};

// ************************************************************************ End Favorite Cast Member ************************************************************************

// ************************************************************************ Sponsors Search Bar Button ***********************************************************************

//Adds functionality to the search bar
$(document).ready(function () {
    $("#Search").on("keyup", function () {                //Allows user to key up and down through previous searches
        var value = $(this).val().toLowerCase();
        $("#sponsor-search tr").filter(function () {
            $(this).toggle($(this).children(":first").text().toLowerCase().indexOf(value) > -1) //this filter looks at the tr and then looks at the first child and matches the text to the search as well as the lowercase makes it to where it doesnt matter if they use captitalzation.
                //($(this).text().toLowerCase().indexOf(value) > -1)
        });
    });
});

        
// ************************************************************************ End Sponsor Search Bar ***************************************************************************

//****************************************************************************** Delete Mulitple Parts Scripts *****************************************************************************************//


// Toggles the trash button above parts to be active, or inactive
$(".partDeleteCheckbox").click(function () {
    var trash = $(".msg-del-btn");
    var id = $(this).attr('id');
    if ($(".partDeleteCheckbox").is(":checked")) {
        trash.removeClass("inactive");
    }
    else {
        trash.addClass("inactive");
    }
    DeleteVisualQueue(id);
});

// Toggles the visual queue to work on only the targeted part
function DeleteVisualQueue(id) {
    var part = $("#" + id);
    var partVal = part.val()
    if (part.is(":checked")) {
        $('#part-card-' + partVal).addClass('shadow-3-sec');
        $('#check-' + partVal).removeClass('part-card-checkbox');
        $('#check-' + partVal).addClass('part-card-checkbox-opaque');
    }
    else {
        $('#part-card-' + partVal).removeClass('shadow-3-sec');
        $('#check-' + partVal).removeClass('part-card-checkbox-opaque');
        $('#check-' + partVal).addClass('part-card-checkbox');
    }

}

// Passing the CalendarEvents id's that are checked to the modal to be sent to the controller for deletion
$(".delete").click(function () {
    var modal = $("#multiDeleteModal").find(".modal-body");
    $(".partDeleteCheckbox:checked").each(function (index) {
        var partId = $(this).val();
        $(modal).append("<input name='PartIdsToDelete' value='" + partId + "' type='hidden' />");
    });
});


// Append the Titles' of the selected parts & the total amount to the modal
$(".msg-del-btn").click(function () {
    var partTitle = "";
    var partsList = [];
    var partsTotal = 0;
    $(".partDeleteCheckbox:checked").each(function () {
        partTitle = $(this).attr("name");
        partsList.push(partTitle);
    });
    for (var i = 0; i < partsList.length; i++) {
        $("#partsList").append("<li class='temp-event'>" + partsList[i] + "</li>");
    }
    partsTotal = partsList.length;
    $(".parts-total").append("<span class='temp-total'>" + partsTotal + "</span>");

});

// Remove the Titles and total number of selected parts from the modal on cancel
$(".cancel").click(function () {
    $("#partsList").children().remove(".temp-event");
    $(".parts-total").children().remove(".temp-total");
});

// Hovers a box when object is hovered only
$(function () {
    $('[data-toggle="part-Delete-Checkbox"]').tooltip({ trigger: "hover" }, {
        'delay': { hide: 300 }
    });
});



//Animation of bug report button//
/*defining global variables so we can use them in the disappear and reappear functions */        
var mybug_icon_txt = document.getElementById("bug_icon_btn_txt");
var mybug_icon_rightarrow = document.getElementById("bug_icon_btn_rightarrow");
var mybug_icon_leftarrow = document.getElementById("bug_icon_btn_leftarrow");

/* function to make the bug report button disappear and disable when the right chevron is clicked*/
function disappear_icon() {
                      
    mybug_icon_rightarrow.style.display = 'none';
    mybug_icon_leftarrow.style.display = 'inline';
}

/*this function brings back the button when the left chevron is clicked.*/
function reappear_icon() {                                   

    mybug_icon_rightarrow.style.display = 'inline';
    mybug_icon_leftarrow.style.display = 'none';
}

//****************************************************************************** Delete Mulitple Parts Scripts End *************************************************************************************//

