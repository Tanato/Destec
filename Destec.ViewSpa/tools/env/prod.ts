import {EnvConfig} from './env-config.interface';

const ProdConfig: EnvConfig = {
  ENV: 'PROD',
  API: 'http://calcularcoreapi.mybluemix.net/api/'
};

export = ProdConfig;

