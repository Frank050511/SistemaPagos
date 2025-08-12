import { Disclosure, DisclosureButton, DisclosurePanel, Menu, MenuButton, MenuItem, MenuItems } from '@headlessui/react'
import { Bars3Icon, BellIcon, XMarkIcon } from '@heroicons/react/24/outline'
import LogoutButton from '../Components/LogoutButton.jsx'


export default function Example() {
  return (
    <>
      <div className="min-h-full">
        <header className="relative bg-gray-800 after:pointer-events-none after:absolute after:inset-x-0 after:inset-y-0 after:border-y after:border-white/10">
                  <div className="mx-auto max-w-7xl px-4 py-6 sm:px-6 lg:px-8 flex justify-between items-center">
                      <h1 className="text-3xl font-bold tracking-tight text-white">Administrador de planillas</h1>
                      <div className="flex items-center space-x-4">
                          {/* Puedes agregar m�s elementos aqu� si necesitas */}
                          <LogoutButton />
                      </div>
                  </div>
        </header>
        <main>
          <div className="mx-auto max-w-7xl px-4 py-6 sm:px-6 lg:px-8">{/* Your content */}</div>
        </main>
      </div>
    </>
  )
}
