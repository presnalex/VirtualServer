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
            TotalUsageTime: '',
            StartTime: new Date(new Date().setHours(0, 0, 0, 0)),
            UsageSecondsFromDB: 0
        };

    },
    mounted: function () {
        axios
            .get(baseUrl + "Home/GetData")
            .then(response => {
                this.items = response.data.DictData.map(elem => function () {
                    return elem;
                }());
                this.UsageSecondsFromDB = response.data.UsageSecondsFromDB;
                this.StartTime = new Date(window.vm.StartTime.getTime() + response.data.UsageSecondsFromDB * 1000);
                this.TotalUsageTime = moment(this.StartTime.getHours() + ':' + this.StartTime.getMinutes() + ':' + this.StartTime.getSeconds(), 'hh:m:ss').format('HH:mm:ss');
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
            if (value && (value.indexOf('-62135596800000') == -1)) {
                return moment(String(new Date(parseInt(value.substr(6))))).format('YYYY-MM-DD HH:mm:ss');
            }
        }

    }
});

var timerID = setInterval(updateTime, 1000);

function updateTime() {
    if (window.vm.items) {
        if (window.vm.items.filter(x => x.RemoveDateTime.indexOf('-62135596800000') != -1).length > 0) {
            window.vm.StartTime = new Date(window.vm.StartTime.getTime() + 1000);
            window.vm.TotalUsageTime = moment(window.vm.StartTime.getHours() + ':' + window.vm.StartTime.getMinutes() + ':' + window.vm.StartTime.getSeconds(), 'hh:m:ss').format('HH:mm:ss');
        }
    }
    
    var CurrDate = new Date();
    window.vm.CurrentDateTime = moment(CurrDate).format('YYYY-MM-DD HH:mm:ss');   
};



