// Inline Page
var pageInput = {
    pageType: "custom",
    name: "ais_createpayrollaccount_bbd5e",
    entityName: "<logical name of the table>",
    recordId: "<record id>",
};
var navigationOptions = {
    target: 2,
    position: 1,
    width: {value: 50, unit: "%"},
    title: "Create Payroll Account"
};
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