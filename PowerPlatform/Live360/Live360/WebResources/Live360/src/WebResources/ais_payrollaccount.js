function openCreatePayrollAccount(primaryItemIds) {
 // Inline Page

 let itemId = primaryItemIds[0];

var pageInput = {
    pageType: "custom",
    name: "ais_createpayrollaccount_bbd5e",
    recordId: itemId,
};
var navigationOptions = {
    target: 2,
    position: 1,
    height: {value: 400, unit: "px"},
    width: {value: 640, unit: "px"},
    title: "Create Payroll Account"
};

console.log(primaryItemIds);

Xrm.Navigation.navigateTo(pageInput, navigationOptions)
    .then(
        function () {
            // Called when page opens
        }
    ).catch(
        function (error) {
            // Handle error
        }
    );
}

