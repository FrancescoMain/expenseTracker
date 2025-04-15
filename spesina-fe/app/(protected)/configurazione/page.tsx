// app/configurazione/page.tsx
"use client";

import BudgetSettings from "@/components/BudgetSettings";
import CategoriaSettings from "@/components/settings/CategoriaSettings";
import SubcategoriaSettings from "@/components/settings/SubcategoriaSettings";
import { useAuth } from "@/context/AuthContext";
import { useRouter } from "next/navigation";
import React from "react";

export default function ConfigurazionePage() {
  const { user, isLoading } = useAuth();
  const router = useRouter();

  React.useEffect(() => {
    if (!isLoading && !user) {
      router.push("/login");
    }
  }, [isLoading, user]);

  if (isLoading || !user) return <div>Caricamento...</div>;
  return (
    <div className="p-8 space-y-8 ">
      <h1 className="text-3xl font-bold col-sec">Configurazione Iniziale</h1>

      <div className="bc-ter rounded-xl p-6 shadow mb-6 border ">
        <h2 className="text-xl font-semibold col-sec flex items-center gap-2 mb-4">
          <span>📦</span> Categorie
        </h2>
        <CategoriaSettings />
      </div>

      <SubcategoriaSettings />
      <BudgetSettings />
    </div>
  );
}
