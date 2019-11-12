import Component from 'vue-class-component';
import { expect } from 'chai';
import { ComponentTest } from '../../util/component-test';
import { ListComponent } from './list';

@Component({
  template: require('./list.html')
})
class MockListComponent extends ListComponent {
  constructor() {
    super();
    this.axios = {
      get: () => {
        return Promise.resolve({ data: [{ name: 'test 1' }, { name: 'test 2' }, { name: 'test 3' }] });
      }
    };
  }
}

describe('List component', () => {
  let directiveTest: ComponentTest;

  beforeEach(() => {
    directiveTest = new ComponentTest('<div><list></list></div>', { 'list': MockListComponent });
  });

  it('should render correct contents', async () => {
    directiveTest.createComponent();

    await directiveTest.execute((vm) => { // ensure Vue has bootstrapped/run change detection
      debugger;
      console.log(vm.$el.querySelectorAll('.content ul li'));
      expect(vm.$el.querySelectorAll('.content ul li').length).to.equal(3);
    });
  });
});
