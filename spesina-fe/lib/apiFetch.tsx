// lib/apiFetch.ts
import router from "next/router";

export const apiFetch = async (
  url: string,
  options: RequestInit = {}
): Promise<Response> => {
  const token = localStorage.getItem("token");

  const headers = {
    ...(options.headers || {}),
    ...(token ? { Authorization: `Bearer ${token}` } : {}),
    "Content-Type": "application/json",
  };
  try {
    const res = await fetch(url, {
      ...options,
      headers,
    });

    if (res.status == 401) {
      console.error("Non autorizzato – redirect alla login");
      localStorage.removeItem("token");
      router.push("/login");
      throw new Error("Non autorizzato – redirect alla login");
    }
    return res;
  } catch (error) {
    localStorage.removeItem("token");
    router.push("/login");
    console.error("Errore durante la fetch:", error);
    throw error;
  }
};
