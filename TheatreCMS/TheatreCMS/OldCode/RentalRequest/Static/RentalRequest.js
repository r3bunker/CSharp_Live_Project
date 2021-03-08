// ************************************************************************ Rental Request Calendar Script *************************************************************************************//

//calendar Icon toggle 

function toggleIcon(x) {
    x.classList.toggle("fa-calendar-plus");
    x.classList.toggle("fa-calendar-minus");
}

//Toggle calendar show/hide
$("#toggleCal").click(function () {
    $("#rentalCalendar").toggle('fast', function () {
        $(".log").text('Toggle Transition Complete');
    });
});

var rentalEvents = [];

//fullCalendar AJAX request for rentalEvent data
GetRentalEvents();
function GetRentalEvents() {
    $.ajax({
        type: "GET",
        url: "/RentalRequest/GetRentalEvents",
        success: function (data) {
            rentalEvents = data
            GenerateCalendar(rentalEvents);
        }
    })
};

function GenerateCalendar(rentalEvents) {
    //remove the previous calendar
    $('#rentalCalendar').fullCalendar('destroy');
    //Assign properties and events to the calendar        
    $('#rentalCalendar').fullCalendar({
        header: {
            left: 'prev,next today',
            center: 'title',
            right: 'month,agendaWeek'
        },
        allDaySlot: false,
        contentHeight: 550,
        timezone: "local",
        defaultDate: new Date(),
        events: rentalEvents,
        eventColor: '#f04d44',
        eventBorderColor: '#fffbfb',
        //pass rentalId parameter to Details method on click
        eventClick: function getDetails(rentalEvents) {
            var requestId = rentalEvents.rentalRequestId;
            window.location.href = ("/rentalRequest/Details/" + requestId);
        }
    });
};  // ************************************************************************ Rental Request Calendar Script End *************************************************************************************//    
