"use client";

import { useEffect, useState } from "react";
import { useAuth } from "@/context/AuthContext";
import { apiFetch } from "@/lib/apiFetch";

export default function BudgetSettings() {
  const { token } = useAuth();
  const [budgets, setBudgets] = useState<any[]>([]);
  const [categories, setCategories] = useState<any[]>([]);
  const [subcategories, setSubcategories] = useState<any[]>([]);

  const [categoryId, setCategoryId] = useState<number | null>(null);
  const [subcategoryId, setSubcategoryId] = useState<number | null>(null);
  const [amount, setAmount] = useState<number>(0);

  const fetchData = async () => {
    const headers = { Authorization: `Bearer ${token}` };
    const b = await (
      await apiFetch("http://localhost:5299/api/Budget", { headers })
    ).json();
    const c = await (
      await apiFetch("http://localhost:5299/api/Category", { headers })
    ).json();
    const s = await (
      await apiFetch("http://localhost:5299/api/Subcategory", { headers })
    ).json();

    setBudgets(b.data);
    setCategories(c.data);
    setSubcategories(s.data);
  };

  const handleCreate = async () => {
    if (!categoryId || amount <= 0) return;

    await apiFetch("http://localhost:5299/api/Budget", {
      method: "POST",
      headers: {
        Authorization: `Bearer ${token}`,
        "Content-Type": "application/json",
      },
      body: JSON.stringify({
        categoryId,
        subcategoryId,
        amount,
        startDate: new Date().toISOString(),
      }),
    });

    setAmount(0);
    setCategoryId(null);
    setSubcategoryId(null);
    fetchData();
  };

  const handleDelete = async (id: number) => {
    await apiFetch(`http://localhost:5299/api/Budget/${id}`, {
      method: "DELETE",
      headers: { Authorization: `Bearer ${token}` },
    });
    fetchData();
  };

  useEffect(() => {
    if (token) fetchData();
  }, [token]);

  const filteredSubs = subcategories.filter((s) => s.categoryId === categoryId);

  return (
    <div className="bc-ter rounded-xl p-6 shadow border">
      <h2 className="text-xl font-semibold col-sec mb-4 flex items-center gap-2">
        <span>📊</span> Budget Categorie
      </h2>

      <div className="flex gap-2 flex-wrap mb-4">
        <select
          className="border p-2 rounded bg-gray-800 text-white"
          value={categoryId ?? ""}
          onChange={(e) => setCategoryId(parseInt(e.target.value))}
        >
          <option value="">Seleziona categoria</option>
          {categories.map((c) => (
            <option key={c.id} value={c.id}>
              {c.name}
            </option>
          ))}
        </select>

        <select
          className="border p-2 rounded bg-gray-800 text-white"
          value={subcategoryId ?? ""}
          onChange={(e) => setSubcategoryId(parseInt(e.target.value))}
        >
          <option value="">Tutte le sottocategorie</option>
          {filteredSubs.map((s) => (
            <option key={s.id} value={s.id}>
              {s.name}
            </option>
          ))}
        </select>

        <input
          type="number"
          className="border p-2 rounded bg-gray-800 text-white"
          placeholder="Importo budget"
          value={amount}
          onChange={(e) => setAmount(parseFloat(e.target.value))}
        />

        <button
          onClick={handleCreate}
          className="bg-green-600 text-white px-4 py-2 rounded"
        >
          Aggiungi
        </button>
      </div>

      <table className="w-full border text-sm">
        <thead className="bg-gray-700 text-white font-semibold">
          <tr>
            <th className="p-2">Categoria</th>
            <th className="p-2">Sottocategoria</th>
            <th className="p-2">Importo</th>
            <th className="p-2">Azioni</th>
          </tr>
        </thead>
        <tbody>
          {budgets.map((b) => (
            <tr key={b.id} className="border-t">
              <td className="p-2">{b.categoryName}</td>
              <td className="p-2">{b.subcategoryName ?? "-"}</td>
              <td className="p-2">€ {b.amount.toFixed(2)}</td>
              <td className="p-2">
                <button
                  onClick={() => handleDelete(b.id)}
                  className="bg-red-500 text-white px-3 py-1 rounded"
                >
                  Elimina
                </button>
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}
