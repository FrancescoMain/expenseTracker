"use client";

import { useAuth } from "@/context/AuthContext";
import { apiFetch } from "@/lib/apiFetch";
import { useEffect, useState } from "react";

interface Transfer {
  id: number;
  amount: number;
  date: string;
  fromAccountName: string;
  toAccountName: string;
  note?: string;
}

export default function TransferPage() {
  const { token } = useAuth();
  const [transfers, setTransfers] = useState<Transfer[]>([]);
  const [fromId, setFromId] = useState<number | null>(null);
  const [toId, setToId] = useState<number | null>(null);
  const [amount, setAmount] = useState(0);
  const [note, setNote] = useState("");
  const [accounts, setAccounts] = useState<any[]>([]);
  const [savingGoals, setSavingGoals] = useState<any[]>([]);
  const [selectedGoalId, setSelectedGoalId] = useState<number | null>(null);

  const fetchTransfers = async () => {
    const res = await apiFetch("http://localhost:5299/api/Transfer", {
      headers: { Authorization: `Bearer ${token}` },
    });
    const data = await res.json();
    setTransfers(data.data);
  };

  const fetchAccounts = async () => {
    const res = await apiFetch("http://localhost:5299/api/Account", {
      headers: { Authorization: `Bearer ${token}` },
    });
    const data = await res.json();
    setAccounts(data.data.accounts);
  };
  const fetchGoals = async () => {
    const res = await apiFetch("http://localhost:5299/api/SavingGoal", {
      headers: { Authorization: `Bearer ${token}` },
    });
    const data = await res.json();
    setSavingGoals(data.data);
  };

  const deleteTransfer = async (id: number) => {
    const confirm = window.confirm(
      "Sei sicuro di voler eliminare questo trasferimento?"
    );
    if (!confirm) return;

    const res = await apiFetch(`http://localhost:5299/api/Transfer/${id}`, {
      method: "DELETE",
      headers: {
        Authorization: `Bearer ${token}`,
      },
    });

    if (res.ok) {
      fetchTransfers();
    } else {
      const data = await res.json();
      alert(data.message || "Errore durante l'eliminazione");
    }
  };

  const handleCreate = async () => {
    await apiFetch("http://localhost:5299/api/Transfer", {
      method: "POST",
      headers: {
        Authorization: `Bearer ${token}`,
        "Content-Type": "application/json",
      },
      body: JSON.stringify({
        fromAccountId: fromId,
        toAccountId: toId,
        amount,
        note,
        savingGoalId: selectedGoalId,
      }),
    });
    setAmount(0);
    setFromId(null);
    setToId(null);
    setNote("");
    fetchTransfers();
  };

  useEffect(() => {
    if (!token) return;
    fetchTransfers();
    fetchAccounts();
    fetchGoals();
  }, [token]);

  useEffect(() => {
    console.log("Accounts:", accounts);
  }, [accounts]);

  useEffect(() => {
    console.log("Selected toId:", fromId);
    console.log(
      "Filtered Goals:",
      savingGoals.filter((g) => g.accountId === fromId)
    );
  }, [toId, savingGoals]);

  const fromAccountName = accounts.find((acc) => acc.id === fromId)?.name;

  return (
    <div className="p-6">
      <h1 className="text-2xl font-bold mb-4">Trasferimenti</h1>

      <div className="mb-6">
        <h2 className="text-lg font-semibold mb-2">Nuovo trasferimento</h2>
        <div className="flex flex-wrap gap-2">
          <select
            className="bg-gray-800 text-white border p-2 rounded"
            value={fromId ?? ""}
            onChange={(e) => setFromId(Number(e.target.value))}
          >
            <option value="">Da conto</option>
            {accounts.map((acc) => (
              <option key={acc.id} value={acc.id}>
                {acc.name}
              </option>
            ))}
          </select>
          <select
            className="bg-gray-800 text-white border p-2 rounded"
            value={toId ?? ""}
            onChange={(e) => setToId(Number(e.target.value))}
          >
            <option value="">A conto</option>
            {accounts.map((acc) => (
              <option key={acc.id} value={acc.id}>
                {acc.name}
              </option>
            ))}
          </select>
          {fromAccountName &&
            savingGoals.some(
              (goal) => goal.accountName === fromAccountName
            ) && (
              <select
                className="bg-gray-800 text-white border p-2 rounded"
                value={selectedGoalId ?? ""}
                onChange={(e) => setSelectedGoalId(Number(e.target.value))}
              >
                <option value="">Collega obiettivo (opzionale)</option>
                {savingGoals
                  .filter((goal) => goal.accountName === fromAccountName)
                  .map((goal) => (
                    <option key={goal.id} value={goal.id}>
                      {goal.name} (€{goal.targetAmount})
                    </option>
                  ))}
              </select>
            )}

          <input
            type="number"
            placeholder="Importo"
            className="bg-gray-800 text-white border p-2 rounded"
            value={amount}
            onChange={(e) => setAmount(parseFloat(e.target.value))}
          />
          <input
            type="text"
            placeholder="Nota"
            className="bg-gray-800 text-white border p-2 rounded"
            value={note}
            onChange={(e) => setNote(e.target.value)}
          />
          <button
            onClick={handleCreate}
            className="bg-green-600 text-white px-4 py-2 rounded"
          >
            Salva
          </button>
        </div>
      </div>

      <h2 className="text-lg font-semibold mt-4 mb-2">Storico trasferimenti</h2>
      <table className="w-full border text-sm">
        <thead className="bg-gray-700 text-white font-semibold">
          <tr>
            <th className="p-2">Data</th>
            <th className="p-2">Da</th>
            <th className="p-2">A</th>
            <th className="p-2">Importo</th>
            <th className="p-2">Nota</th>
            <th className="p-2">Azioni</th>
          </tr>
        </thead>
        <tbody>
          {transfers.map((t) => (
            <tr key={t.id} className="border-t">
              <td className="p-2">{new Date(t.date).toLocaleDateString()}</td>
              <td className="p-2">{t.fromAccountName}</td>
              <td className="p-2">{t.toAccountName}</td>
              <td className="p-2 text-blue-500 font-semibold">
                € {t.amount.toFixed(2)}
              </td>
              <td className="p-2">{t.note ?? "-"}</td>
              <td className="p-2">
                <button
                  onClick={() => deleteTransfer(t.id)}
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
