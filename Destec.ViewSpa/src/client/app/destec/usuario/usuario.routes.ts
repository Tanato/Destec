import { Route } from '@angular/router';

import { UsuarioMasterComponent } from './index';
import { UsuarioDetailComponent } from './index';
import { UsuarioAlterarSenhaComponent } from './index';


export const UsuarioRoutes: Route[] = [
	{
		path: 'usuario',
		component: UsuarioMasterComponent
	},
    {
		path: 'usuario/detail',
		component: UsuarioDetailComponent
	},
	{
		path: 'usuario/alterarSenha',
		component: UsuarioAlterarSenhaComponent
	},
];
