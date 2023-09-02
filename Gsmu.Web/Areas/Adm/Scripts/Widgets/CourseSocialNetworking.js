var facebookAPI = function () {
    return {
        managedPageList: [],
        checkLoginStatus: function () {
            FB.getLoginStatus(function (response) {
                if (response.status === 'connected') {
                    courseFacebook.handleConnected();
                    courseFacebook.handleLogInUI();
                    facebookAPI.loadManagedPageList(facebookAPI.managedPageList);
                }
                else {
                    courseFacebook.handleDisconnected();
                    courseFacebook.handleLogOutUI();
                }
                $('#facebookButtons').show();
            });
        },
        login: function () {
            FB.login(function (response) {
                facebookAPI.checkLoginStatus();
            }, { scope: 'manage_pages, publish_pages' });
        },
        logout: function () {
            FB.logout(function (response) {
                courseFacebook.handleDisconnected();
                courseFacebook.handleLogOutUI();
            });
        },
        getListOfManagedPage: function (callback) {
            var managedPageList = [];
            FB.api('/me/accounts/', 'GET', {}, function (response) {
                if (response && !response.error) {
                    var faceBookData = response.data;
                    faceBookData.map(function (data) {
                        if (data) {
                            managedPageList.push({ id: data.id, name: data.name });
                        }
                    });
                    //do a callback after data has been added to the array
                    if (typeof callback === 'function') {
                        callback(managedPageList);
                    }
                }
            });
            return managedPageList;
        },
        loadManagedPageList: function (managedPageList) {
            //call the api
            facebookAPI.getListOfManagedPage(function (managedPageList) {
                var html = '';;
                if (managedPageList.length > 0) {
                    managedPageList.map(function (data) {
                        html += '<option value="' + data.id + '">' + data.name + '</option>';
                    });
                }
                var defaultTemplate = '<option value="">Default Wall</option>';
                html += defaultTemplate
                $('#fbpageslist').html(html);
            });
        },
        shareToWall: function () {
            var pageid = $('#fbpageslist option:selected').val();
            var linkURL = globalObject.dotNetSiteRootUrl + '/Public/Course/Browse#{"CourseActiveState":"All","Text":"' + courseMain.courseId + '","CoursePopout":' + courseMain.courseId +'}';
            FB.ui({
                method: 'feed',
                display: 'popup',
                picture: courseMain.tileImageUrl && courseMain.tileImageUrl != '' ? courseMain.tileImageUrl : '',
                name: courseMain.courseNumber + ' ' + courseMain.courseName,
                link: linkURL,
                caption: courseMain.isOnline === 1 ? 'Online' : 'Start Date : (' + courseDateTimes.minDate +')',
                description: courseMain.description,
                message: 'Post Additional Message/Commen on your wall.',
                from: pageid,
                to: pageid
            });
        }
    }
}();

var courseFacebook = {
    apiKey: globalObject.fbApiKey, //for testing - load this from db (masterinfo3.facebookAPInum)
    init: function () {
        window.fbAsyncInit = function () {
            FB.init({
                appId: courseFacebook.apiKey,
                status: true,
                xfbml: true,
                version: 'v2.2'
            });
            facebookAPI.checkLoginStatus();
        };
        (function (d, s, id) {
            var js, fjs = d.getElementsByTagName(s)[0];
            if (d.getElementById(id)) { return; }
            js = d.createElement(s); js.id = id;
            js.src = "//connect.facebook.net/en_US/sdk.js";
            fjs.parentNode.insertBefore(js, fjs);
        }(document, 'script', 'facebook-jssdk'));
    },
    handleLogInUI: function () {
        $('#loginButton').hide();
        $('#logoutButton').show();
    },
    handleLogOutUI: function () {
        $('#loginButton').show();
        $('#logoutButton').hide();
    },
    handleConnected: function () {
        $('#pagecontainer').show();
        $('#pagecontainerlabel').hide();
        $('#pagelink').hide();

        return false;
    },
    handleDisconnected: function () {
        alert("You must login to your Facebook account first");
        $('#pagecontainer').hide();
        $('#pagecontainerlabel').hide();
        $('#pagelink').hide();
        return false;
    },
    navigateToCourseComments: function () {
        window.open(gsmuObject.adminUrl + 'systemconfig_courseComments.asp', '_blank');
    }
}
$(document).ready(function () {
    courseFacebook.init();
})