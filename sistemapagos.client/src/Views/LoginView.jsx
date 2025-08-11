import "../index.css";

export default function Login() {
    return (
        <>
            <div className="flex min-h-full flex-col justify-center px-6 py-12 lg:px-8">
                <div className="sm:mx-auto sm:w-full sm:max-w-sm">
                    <h2 className="mt-10 text-center text-2xl/9 font-bold tracking-tight text-white">Ingresa a tu cuenta</h2>
                </div>

                <div className="mt-10 sm:mx-auto sm:w-full sm:max-w-sm">
                    <form action="" method="POST" className="space-y-6">{/* en action se pondrá la direccion a donde se enviaran los datos, en este caso seria al modelo o algo asi: "/api/login" */}
                        <div>
                            <label htmlFor="codigoEmpleado" className="block text-sm/6 font-medium text-gray-100">Codigo de Empleado: </label>
                            <div className="mt-2">
                                <input id="codigoEmpleado"
                                    name="codigoEmpleado"
                                    type="text" required
                                    autoComplete="username"
                                    className="block w-full rounded-md bg-white/5 px-3 py-1.5 text-base text-white outline-1 -outline-offset-1 outline-white/10 placeholder:text-gray-500 focus:outline-2 focus:-outline-offset-2 focus:outline-indigo-500 sm:text-sm/6"
                                />
                            </div>
                        </div>
                        <div>
                            <div className="flex items-center justify-between">
                                <label htmlFor="clave" className="block text-sm/6 font-medium text-gray-100">Clave: </label>
                                <div className="text-sm">
                                    <a href="#" className="font-semibold text-indigo-400 hover:text-indigo-300">
                                        Olvidaste tu clave?
                                    </a>
                                </div>
                            </div>
                            <div className="mt-2">
                                <input
                                    id="clave"
                                    name="clave"
                                    type="password"
                                    required
                                    autoComplete="current-password"
                                    className="block w-full rounded-md bg-white/5 px-3 py-1.5 text-base text-white outline-1 -outline-offset-1 outline-white/10 placeholder:text-gray-500 focus:outline-2 focus:-outline-offset-2 focus:outline-indigo-500 sm:text-sm/6"
                                />
                            </div>
                        </div>
                        <div>
                            <button type="submit" className="flex w-full justify-center rounded-md bg-indigo-500 px-3 py-1.5 text-sm/6 font-semibold text-white hover:bg-indigo-400 focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-indigo-500">
                                Ingresar
                            </button>
                        </div>
                    </form>
                </div>
            </div>
        </>
    )
}