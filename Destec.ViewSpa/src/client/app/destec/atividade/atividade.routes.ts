import { Route } from '@angular/router';

import { AtividadeMasterComponent } from './index';
import { AtividadeExecucaoComponent } from './index';

export const AtividadeRoutes: Route[] = [
	{
		path: 'atividade/master',
		component: AtividadeMasterComponent
	},
    {
		path: 'atividade/execucao',
		component: AtividadeExecucaoComponent
	},
];
