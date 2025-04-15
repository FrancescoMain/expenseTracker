"use client";

import { useEffect, useState } from "react";
import { useAuth } from "@/context/AuthContext";
import { apiFetch } from "@/lib/apiFetch";

interface SavingGoal {
  id: number;
  name: string;
  targetAmount: number;
  currentAmount: number;
  accountName: string;
}

export default function SavingGoalsPage() {
  const { token } = useAuth();
  const [goals, setGoals] = useState<SavingGoal[]>([]);
  const [name, setName] = useState("");
  const [targetAmount, setTargetAmount] = useState(0);
  const [accountId, setAccountId] = useState<number | null>(null);
  const [accounts, setAccounts] = useState<any[]>([]);

  const fetchGoals = async () => {
    const res = await apiFetch("http://localhost:5299/api/SavingGoal", {
      headers: { Authorization: `Bearer ${token}` },
    });
    const data = await res.json();
    setGoals(data.data);
  };

  const fetchAccounts = async () => {
    const res = await apiFetch("http://localhost:5299/api/Account", {
      headers: { Authorization: `Bearer ${token}` },
    });
    const data = await res.json();
    setAccounts(data.data.accounts); // attenzione se il backend incapsula sotto `accounts`
  };

  const handleCreate = async () => {
    await apiFetch("http://localhost:5299/api/SavingGoal", {
      method: "POST",
      headers: {
        Authorization: `Bearer ${token}`,
        "Content-Type": "application/json",
      },
      body: JSON.stringify({ name, targetAmount, accountId }),
    });

    setName("");
    setTargetAmount(0);
    setAccountId(null);
    fetchGoals();
  };

  const handleDelete = async (id: number) => {
    await apiFetch(`http://localhost:5299/api/SavingGoal/${id}`, {
      method: "DELETE",
      headers: { Authorization: `Bearer ${token}` },
    });
    fetchGoals();
  };

  useEffect(() => {
    if (token) {
      fetchGoals();
      fetchAccounts();
    }
  }, [token]);

  return (
    <div className="p-6">
      <h1 className="text-2xl font-bold mb-6">🎯 Obiettivi di Risparmio</h1>

      <div className="mb-6 space-y-2">
        <h2 className="text-lg font-semibold">Nuovo Obiettivo</h2>
        <input
          type="text"
          placeholder="Nome obiettivo"
          className="border p-2 rounded mr-2"
          value={name}
          onChange={(e) => setName(e.target.value)}
        />
        <input
          type="number"
          placeholder="Importo obiettivo"
          className="border p-2 rounded mr-2"
          value={targetAmount}
          onChange={(e) => setTargetAmount(Number(e.target.value))}
        />
        <select
          className="border p-2 rounded mr-2"
          value={accountId ?? ""}
          onChange={(e) => setAccountId(Number(e.target.value))}
        >
          <option value="">Seleziona conto</option>
          {accounts.map((acc) => (
            <option key={acc.id} value={acc.id}>
              {acc.name}
            </option>
          ))}
        </select>
        <button
          onClick={handleCreate}
          className="bg-green-600 hover:bg-green-700 text-white px-4 py-2 rounded"
        >
          Salva
        </button>
      </div>

      <h2 className="text-lg font-semibold mb-2">📋 Lista Obiettivi</h2>
      <table className="w-full border text-sm">
        <thead className="bg-gray-700 text-white">
          <tr>
            <th className="p-2">Nome</th>
            <th className="p-2">Conto</th>
            <th className="p-2">Obiettivo</th>
            <th className="p-2">Risparmiato</th>
            <th className="p-2">Avanzamento</th>
            <th className="p-2">Azioni</th>
          </tr>
        </thead>
        <tbody>
          {goals.map((g) => (
            <tr key={g.id} className="border-t">
              <td className="p-2">{g.name}</td>
              <td className="p-2">{g.accountName}</td>
              <td className="p-2">€ {g.targetAmount.toFixed(2)}</td>
              <td className="p-2">€ {g.currentAmount.toFixed(2)}</td>
              <td className="p-2">
                <div className="bg-gray-300 rounded-full h-3 overflow-hidden">
                  <div
                    className="bg-green-500 h-3"
                    style={{
                      width: `${Math.min(
                        (g.currentAmount / g.targetAmount) * 100,
                        100
                      )}%`,
                    }}
                  ></div>
                </div>
              </td>
              <td className="p-2">
                <button
                  onClick={() => handleDelete(g.id)}
                  className="bg-red-600 hover:bg-red-700 text-white px-3 py-1 rounded"
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
