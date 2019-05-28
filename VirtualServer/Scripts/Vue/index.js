var Vue = require('vue/dist/vue.js');
var axios = require('axios');
var moment = require('moment');
require("@babel/polyfill");


window.axios = axios;
window.Vue = Vue;

window.vm = new Vue({
    el: '#app',
    data() {
        return {
            items: null,
            loading: true,
            errored: false,
            CurrentDateTime: '',
            TotalUsageTime: ''
        };

    },
    mounted: function () {
        axios
            .get(baseUrl + "Home/GetData")
            .then(response => {
                this.items = response.data.map(elem => function () {
                    return elem;
                }());

            })
            .catch(error => {
                console.log(error);
                this.errored = true;
            })
            .finally(() => (this.loading = false));
    },
    methods: {
        createRow: function () {
            axios
                .post(baseUrl + "Home/CreateRow")
                .then(response => {
                    this.items = response.data.map(elem => function () {
                        return elem;
                    }());

                })
                .catch(error => {
                    console.log(error);
                    this.errored = true;
                })
                .finally(() => (this.loading = false));
        },

        updateData: function () {
            console.log(this.items);
            axios
                .post(baseUrl + "Home/UpdateData", { dataForUpdate: JSON.stringify(this.items) })
                .then(response => {
                    this.items = response.data.map(elem => function () {
                        return elem;
                    }());

                })
                .catch(error => {
                    console.log(error);
                    this.errored = true;
                })
                .finally(() => (this.loading = false));

        },
        formatDate: function (value) {
            if (value) {
                return moment(String(value), 'DD.MM.YYYY HH:mm:ss').format('YYYY-MM-DD HH:mm:ss');
            }
        }

    }
});

var timerID = setInterval(updateTime, 1000);
updateTime();
function updateTime() {
    var cd = new Date();
    window.vm.TotalUsageTime = moment(cd.getHours() + ':' + cd.getMinutes() + ':' + cd.getSeconds(), 'hh:m:ss').format('HH:mm:ss');
};



