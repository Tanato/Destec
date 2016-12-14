import { Route } from '@angular/router';

import { FuncionarioMasterComponent } from './index';
import { FuncionarioDetailComponent } from './index';

export const FuncionarioRoutes: Route[] = [
	{
		path: 'funcionario/cadastro',
		component: FuncionarioMasterComponent
	},
    {
		path: 'funcionario/detail',
		component: FuncionarioDetailComponent
	},
];
