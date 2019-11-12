import Vue from 'vue';
import Component from 'vue-class-component';
import axios, {AxiosResponse} from 'axios';

interface UserResponse {
    id: string;
    name: string;
}

@Component({
    template: require('./list.html')
})
export class ListComponent extends Vue {

    items: UserResponse[] = [];
    private url = 'https://jsonplaceholder.typicode.com/users';
    protected axios;

    constructor() {
      super();
      this.axios = axios;
    }

    mounted() {
        this.$nextTick(() => {
            this.loadItems();
        });
    }

    private loadItems() {
        if (!this.items.length) {
            this.axios.get(this.url).then((response: AxiosResponse) => {
                this.items = response.data;
            }, (error) => {
                console.error(error);
            });
        }
    }
}
