import {EnvConfig} from './env-config.interface';

const ProdConfig: EnvConfig = {
  ENV: 'PROD',
  API: 'http://192.168.0.103:5000/api/'
};

export = ProdConfig;

