// components/Sidebar.tsx
"use client";

import { useAuth } from "@/context/AuthContext";
import { useRouter } from "next/navigation";

export default function Sidebar() {
  const { logout } = useAuth();
  const router = useRouter();

  return (
    <div className="h-screen w-64  text-white flex flex-col p-4">
      <div className="text-2xl font-bold mb-6 col-sec">💸 Spesina</div>

      <nav className="flex-1 col-sec">
        <ul className="space-y-4">
          <li>
            <a href="/dashboard" className="block nav-link rounded px-3 py-2">
              Dashboard
            </a>
          </li>
          <li>
            <a href="/accounts" className="block nav-link rounded px-3 py-2">
              Conti
            </a>
          </li>
          <li>
            <a
              href="/configurazione"
              className="block nav-link rounded px-3 py-2"
            >
              Configurazione
            </a>
          </li>
          <li>
            <a
              href="/tracker"
              className="block nav-link rounded px-3 py-2 transition"
            >
              Budget Tracker
            </a>
          </li>
          <li>
            <a
              href="/report"
              className="block nav-link rounded px-3 py-2 transition"
            >
              Report
            </a>
          </li>
          <li>
            <a
              href="/saving-goals"
              className="block nav-link rounded px-3 py-2 transition"
            >
              Risparmi
            </a>
          </li>
          <li>
            <a
              href="/transfers"
              className="block nav-link rounded px-3 py-2 transition"
            >
              Trasferimenti
            </a>
          </li>
        </ul>
      </nav>

      <button
        onClick={() => {
          logout();
          router.push("/login");
        }}
        className="bg-red-600 hover:bg-red-700 mt-auto py-2 px-4 rounded"
      >
        Logout
      </button>
    </div>
  );
}
