import { Route } from '@angular/router';

import { KitRoutes } from './kit/index';
import { FuncionarioRoutes } from './funcionario/index';
import { PedidoRoutes } from './pedido/index';
import { AtividadeRoutes } from './atividade/index';

import { DestecComponent } from './index';

export const DestecRoutes: Route[] = [
	{
		path: 'destec',
		component: DestecComponent,
		children: [
			...KitRoutes,
			...FuncionarioRoutes,
			...PedidoRoutes,
			...AtividadeRoutes,
		]
	}
];
