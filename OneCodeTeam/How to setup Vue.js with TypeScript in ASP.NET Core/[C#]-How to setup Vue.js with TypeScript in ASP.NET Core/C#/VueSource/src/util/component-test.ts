import Vue from 'vue';
import { SinonSpy } from 'sinon';
import merge from 'lodash.merge';
import { ILogger } from './log';

export interface IComponents {
  [key: string]: Vue.Component;
}

export class ComponentTest {

  public vm: Vue;

  constructor(private template: string, private components: IComponents) {
  }

  public createComponent(createOptions?: any): void {
    let options = {
      template: this.template,
      components: this.components
    };
    if (createOptions) merge(options, createOptions);
    this.vm = new Vue(options).$mount();
  }

  public async execute(callback: (vm: Vue) => Promise<void> | void): Promise<void> {
    await Vue.nextTick();
    await callback(this.vm);
  }

}

export class MockLogger implements ILogger {

  constructor(private loggerSpy: SinonSpy) {
  }

  info(msg: any) {
    this.loggerSpy(msg);
  }

  warn(msg: any) {
    this.loggerSpy(msg);
  }

  error(msg: any) {
    this.loggerSpy(msg);
  }
}
