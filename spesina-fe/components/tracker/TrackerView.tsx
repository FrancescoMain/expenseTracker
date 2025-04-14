"use client";

import { useEffect, useState } from "react";
import { format, parseISO } from "date-fns";
import { useAuth } from "@/context/AuthContext";
import NuovaTransazioneForm from "@/components/tracker/NuovaTransazioneForm";
import { apiFetch } from "@/lib/apiFetch";

interface Transaction {
  id: number;
  amount: number;
  description: string;
  date: string;
  subcategoryId?: number;
  subcategoryName?: string;
  categoryName?: string;
  accountName?: string;
}

export default function TrackerView() {
  const { token } = useAuth();
  const [totalBalance, setTotalBalance] = useState(0);
  const [transactions, setTransactions] = useState<Transaction[]>([]);
  const [editingTransaction, setEditingTransaction] =
    useState<Transaction | null>(null);
  const [selectedMonth, setSelectedMonth] = useState(() =>
    new Date().toISOString().slice(0, 7)
  );

  const fetchTransactions = async () => {
    if (!token) return;
    const res = await apiFetch(
      `http://localhost:5299/api/Transaction?month=${selectedMonth}`,
      {
        headers: { Authorization: `Bearer ${token}` },
      }
    );
    const data = await res.json();
    setTransactions(data.data);
  };

  const fetchBalance = async () => {
    const res = await apiFetch("http://localhost:5299/api/Account", {
      headers: { Authorization: `Bearer ${token}` },
    });
    const data = await res.json();
    setTotalBalance(data.data.totalBalance);
  };

  const totalsPerCategory = transactions.reduce((acc, t) => {
    if (!t.categoryName) return acc;
    acc[t.categoryName] = (acc[t.categoryName] || 0) + t.amount;
    return acc;
  }, {} as Record<string, number>);

  const deleteTransaction = async (id: number) => {
    const res = await fetch(`http://localhost:5299/api/Transaction/${id}`, {
      method: "DELETE",
      headers: {
        Authorization: `Bearer ${token}`,
      },
    });

    if (res.ok) {
      setTransactions((prev) => prev.filter((t) => t.id !== id));
      setTotalBalance(
        (prev) => prev - transactions.find((t) => t.id === id)?.amount!
      );
    }
  };

  const onSaved = async () => {
    await fetchTransactions(); // aggiorna transazioni
    await fetchBalance(); // aggiorna saldo e accounts
  };

  useEffect(() => {
    fetchTransactions();
    fetchBalance();
  }, [selectedMonth, token]);

  const total = transactions.reduce((acc, t) => acc + t.amount, 0);

  return (
    <div className="p-6 tracker">
      <h1 className="text-2xl font-bold mb-4">Spese Mensili</h1>
      <h2 className="text-xl font-bold mb-2">
        Totale disponibile: € {totalBalance.toFixed(2)}
      </h2>

      {/* Selettore mese */}
      <div className="mb-4">
        <label className="block mb-1 font-medium">Mese:</label>
        <input
          type="month"
          className="bg-gray-800 text-white border p-2 rounded"
          value={selectedMonth}
          onChange={(e) => setSelectedMonth(e.target.value)}
        />
      </div>

      {/* Form nuova transazione */}
      <NuovaTransazioneForm
        onSaved={onSaved}
        editingTransaction={editingTransaction}
        clearEditing={() => setEditingTransaction(null)}
      />

      {/* Tabella transazioni */}
      <h2 className="text-xl font-semibold mt-6 mb-2">Spese del mese</h2>
      <table className="w-full border text-sm">
        <thead className="bg-gray-700 text-white font-semibold">
          <tr>
            <th className="p-2">Data</th>
            <th className="p-2">Importo</th>
            <th className="p-2">Categoria</th>
            <th className="p-2">Sottocategoria</th>
            <th className="p-2">Conto</th>
            <th className="p-2">Descrizione</th>
            <th className="p-2">Azioni</th>
          </tr>
        </thead>

        <tbody>
          {transactions.map((t) => (
            <tr key={t.id} className="border-t">
              <td className="p-2">{format(parseISO(t.date), "dd/MM/yyyy")}</td>
              <td className="p-2 text-red-600 font-semibold">
                € {t.amount.toFixed(2)}
              </td>
              <td className="p-2">{t.categoryName || "-"}</td>
              <td className="p-2">{t.subcategoryName || "-"}</td>
              <td className="p-2">{t.accountName || "-"}</td>
              <td className="p-2">{t.description}</td>
              <td className="p-2">
                <button
                  onClick={() => setEditingTransaction(t)}
                  className="bg-yellow-500 hover:bg-yellow-600 text-white px-3 py-1 rounded mr-2"
                >
                  Modifica
                </button>
                <button
                  onClick={() => deleteTransaction(t.id)}
                  className="bg-red-600 hover:bg-red-700 text-white px-3 py-1 rounded"
                >
                  Elimina
                </button>
              </td>
            </tr>
          ))}
        </tbody>
      </table>

      {/* Totale */}
      <div className="mt-8">
        <h3 className="text-xl font-semibold mb-2">Totale per categoria:</h3>
        <ul className="list-disc pl-5">
          {Object.entries(totalsPerCategory).map(([category, total]) => (
            <li key={category}>
              <span className="font-medium">{category}:</span> €{" "}
              {total.toFixed(2)}
            </li>
          ))}
          <li className="font-bold mt-2">
            Totale spese:{" "}
            <span className="text-red-600">€ {total.toFixed(2)}</span>
          </li>
        </ul>
      </div>
    </div>
  );
}
